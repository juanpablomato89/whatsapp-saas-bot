using System;
using System.Collections.Generic;
using WhatsAppSaaS.Domain.Common;
using WhatsAppSaaS.Domain.Enums;
using WhatsAppSaaS.Domain.Events;

namespace WhatsAppSaaS.Domain.Entities
{
    /// <summary>
    /// Tenant = un negocio cliente del SaaS.
    /// Ejemplo: "Restaurante El Cubano", "Salón María", "Tienda Don Pedro".
    /// Es la raíz del agregado principal del sistema multi-tenant.
    /// Todo dato del sistema pertenece a un Tenant.
    /// </summary>
    public class Tenant : AggregateRoot
    {
        // ─── Propiedades ───────────────────────────────────────────────

        /// <summary>Nombre del negocio. Ej: "Restaurante El Cubano".</summary>
        public string BusinessName { get; private set; } = string.Empty;

        /// <summary>Email de contacto del dueño del negocio.</summary>
        public string Email { get; private set; } = string.Empty;

        /// <summary>Número de teléfono del negocio (formato E.164). Ej: +17865551234</summary>
        public string PhoneNumber { get; private set; } = string.Empty;

        /// <summary>País del negocio. Ej: "US", "MX", "CU".</summary>
        public string CountryCode { get; private set; } = string.Empty;

        /// <summary>Zona horaria del negocio. Ej: "America/New_York".</summary>
        public string TimeZone { get; private set; } = "UTC";

        /// <summary>Plan de suscripción actual.</summary>
        public SubscriptionPlan Plan { get; private set; } = SubscriptionPlan.Starter;

        /// <summary>Estado del tenant en el sistema.</summary>
        public TenantStatus Status { get; private set; } = TenantStatus.Active;

        /// <summary>
        /// Número de WhatsApp conectado a este tenant.
        /// Un tenant = un número de WhatsApp Business.
        /// </summary>
        public string? WhatsAppPhoneNumberId { get; private set; }

        /// <summary>Token de acceso de la WhatsApp Business API para este tenant.</summary>
        public string? WhatsAppAccessToken { get; private set; }

        // ─── Navegación (EF Core) ──────────────────────────────────────

        /// <summary>Usuarios (dueños/operadores) que administran este negocio.</summary>
        public ICollection<User> Users { get; private set; } = new List<User>();

        /// <summary>Conversaciones de WhatsApp de este negocio.</summary>
        public ICollection<Conversation> Conversations { get; private set; } = new List<Conversation>();

        // ─── Constructor privado (requerido por EF Core) ───────────────
        private Tenant() { }

        // ─── Factory Method ────────────────────────────────────────────

        /// <summary>
        /// Crea un nuevo tenant con validaciones básicas.
        /// Patrón Factory Method — centraliza la creación y dispara el evento de dominio.
        /// </summary>
        public static Tenant Create(string businessName, string email,
            string phoneNumber, string countryCode, string timeZone)
        {
            if (string.IsNullOrWhiteSpace(businessName))
                throw new ArgumentException("El nombre del negocio es requerido.", nameof(businessName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es requerido.", nameof(email));

            var tenant = new Tenant
            {
                TenantId = Guid.NewGuid(), // El TenantId es su propio Id en el agregado raíz
                BusinessName = businessName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PhoneNumber = phoneNumber.Trim(),
                CountryCode = countryCode.Trim().ToUpperInvariant(),
                TimeZone = timeZone
            };

            // Aseguramos que el TenantId sea su propio Id
            tenant.TenantId = tenant.Id;

            // Disparamos el evento de dominio — otros servicios reaccionarán a esto
            tenant.AddDomainEvent(new TenantCreatedEvent(tenant.Id, tenant.BusinessName, tenant.Email));

            return tenant;
        }

        // ─── Comportamientos del dominio ───────────────────────────────

        /// <summary>Conecta el número de WhatsApp Business a este tenant.</summary>
        public void ConnectWhatsApp(string phoneNumberId, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(phoneNumberId))
                throw new ArgumentException("El PhoneNumberId de WhatsApp es requerido.");

            WhatsAppPhoneNumberId = phoneNumberId;
            WhatsAppAccessToken = accessToken;
            MarkAsUpdated();

            AddDomainEvent(new TenantWhatsAppConnectedEvent(Id, phoneNumberId));
        }

        /// <summary>Cambia el plan de suscripción del tenant.</summary>
        public void ChangePlan(SubscriptionPlan newPlan)
        {
            Plan = newPlan;
            MarkAsUpdated();
        }

        /// <summary>Suspende el tenant (ej: pago vencido).</summary>
        public void Suspend()
        {
            Status = TenantStatus.Suspended;
            MarkAsUpdated();
        }

        /// <summary>Reactiva un tenant suspendido.</summary>
        public void Activate()
        {
            Status = TenantStatus.Active;
            MarkAsUpdated();
        }
    }
}
