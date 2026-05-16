using System.Collections.Generic;
using WhatsAppSaaS.Domain.Common;

namespace WhatsAppSaaS.Domain.Common
{
    /// <summary>
    /// Extiende BaseEntity con soporte para Domain Events.
    /// Los Domain Events permiten que otras partes del sistema
    /// reaccionen a cambios en el dominio sin acoplamiento directo.
    /// Patrón: Observer / Domain Events (DDD).
    /// </summary>
    public abstract class AggregateRoot : BaseEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Lista de eventos pendientes de despachar.
        /// Son procesados por MediatR al finalizar la transacción.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>Registra un nuevo evento de dominio.</summary>
        protected void AddDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        /// <summary>
        /// Limpia los eventos después de ser despachados.
        /// Llamado por la infraestructura al hacer SaveChanges.
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
