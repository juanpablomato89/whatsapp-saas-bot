using System;
using System.Collections.Generic;
using WhatsAppSaaS.Domain.Common;
using WhatsAppSaaS.Domain.Enums;
using WhatsAppSaaS.Domain.Events;

namespace WhatsAppSaaS.Domain.Entities
{
    /// <summary>
    /// Representa una conversación de WhatsApp entre un cliente final
    /// y el bot de un negocio (tenant).
    /// Una conversación agrupa todos los mensajes de un mismo usuario.
    /// </summary>
    public class Conversation : AggregateRoot
    {
        // ─── Propiedades ───────────────────────────────────────────────

        /// <summary>
        /// Número de WhatsApp del cliente final (formato E.164).
        /// Ej: +17865559876. Es el identificador único del interlocutor.
        /// </summary>
        public string CustomerPhoneNumber { get; private set; } = string.Empty;

        /// <summary>Nombre del cliente si fue obtenido de WhatsApp.</summary>
        public string? CustomerName { get; private set; }

        /// <summary>Estado actual de la conversación.</summary>
        public ConversationStatus Status { get; private set; } = ConversationStatus.BotHandling;

        /// <summary>
        /// Fecha y hora del último mensaje recibido.
        /// Usado para expirar sesiones inactivas en Redis.
        /// </summary>
        public DateTime LastMessageAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Indica si el dueño del negocio tomó control manual de la conversación.
        /// Cuando es true, el bot no responde automáticamente.
        /// </summary>
        public bool IsHandledByHuman { get; private set; }

        // ─── Navegación ────────────────────────────────────────────────

        /// <summary>Mensajes que conforman esta conversación.</summary>
        public ICollection<Message> Messages { get; private set; } = new List<Message>();

        // ─── Constructor privado (EF Core) ─────────────────────────────
        private Conversation() { }

        // ─── Factory Method ────────────────────────────────────────────

        /// <summary>
        /// Crea una nueva conversación cuando llega el primer mensaje
        /// de un cliente que no tiene conversación activa.
        /// </summary>
        public static Conversation Create(Guid tenantId, string customerPhoneNumber, string? customerName = null)
        {
            if (string.IsNullOrWhiteSpace(customerPhoneNumber))
                throw new ArgumentException("El número de teléfono del cliente es requerido.");

            var conversation = new Conversation
            {
                TenantId = tenantId,
                CustomerPhoneNumber = customerPhoneNumber.Trim(),
                CustomerName = customerName?.Trim()
            };

            conversation.AddDomainEvent(new ConversationStartedEvent(
                conversation.Id, tenantId, customerPhoneNumber));

            return conversation;
        }

        // ─── Comportamientos del dominio ───────────────────────────────

        /// <summary>
        /// Marca la conversación como escalada a humano.
        /// El bot dejará de responder automáticamente.
        /// </summary>
        public void EscalateToHuman()
        {
            IsHandledByHuman = true;
            Status = ConversationStatus.HumanHandling;
            MarkAsUpdated();

            AddDomainEvent(new ConversationEscalatedEvent(Id, TenantId, CustomerPhoneNumber));
        }

        /// <summary>Devuelve el control al bot.</summary>
        public void ReturnToBot()
        {
            IsHandledByHuman = false;
            Status = ConversationStatus.BotHandling;
            MarkAsUpdated();
        }

        /// <summary>Cierra la conversación.</summary>
        public void Close()
        {
            Status = ConversationStatus.Closed;
            MarkAsUpdated();
        }

        /// <summary>Actualiza la fecha del último mensaje (llamado al recibir o enviar un mensaje).</summary>
        public void RegisterActivity()
        {
            LastMessageAt = DateTime.UtcNow;
            MarkAsUpdated();
        }
    }
}
