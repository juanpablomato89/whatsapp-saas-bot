using MediatR;

namespace WhatsAppSaaS.Domain.Common
{
    /// <summary>
    /// Interfaz marcadora para todos los Domain Events del sistema.
    /// Implementa INotification de MediatR para ser despachados
    /// automáticamente al finalizar cada transacción.
    /// </summary>
    public interface IDomainEvent : INotification { }
}
