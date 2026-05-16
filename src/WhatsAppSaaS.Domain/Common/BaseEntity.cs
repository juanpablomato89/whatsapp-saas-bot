using System;

namespace WhatsAppSaaS.Domain.Common
{
    /// <summary>
    /// Clase base para todas las entidades del dominio.
    /// Provee identidad única, auditoría de creación/modificación
    /// y soporte para multi-tenancy (TenantId).
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Identificador único de la entidad (GUID).
        /// Se genera automáticamente al crear la entidad.
        /// </summary>
        public Guid Id { get; protected set; } = Guid.NewGuid();

        /// <summary>
        /// Identificador del tenant (negocio) al que pertenece esta entidad.
        /// Es el pilar del aislamiento de datos en el sistema multi-tenant.
        /// </summary>
        public Guid TenantId { get; protected set; }

        /// <summary>Fecha y hora UTC en que se creó la entidad.</summary>
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        /// <summary>Fecha y hora UTC de la última modificación. Null si nunca fue modificada.</summary>
        public DateTime? UpdatedAt { get; protected set; }

        /// <summary>
        /// Marca la entidad como modificada y actualiza la fecha.
        /// Debe llamarse en cada operación de actualización del dominio.
        /// </summary>
        protected void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;
    }
}
