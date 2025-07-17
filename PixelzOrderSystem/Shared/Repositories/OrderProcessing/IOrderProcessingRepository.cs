using PixelzOrderSystem.Shared.Domain.Entities;

namespace PixelzOrderSystem.Shared.Repositories.OrderProcessing;

public interface IOrderProcessingRepository
{
    Task<OrderProcessingSaga?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddSagaAsync(OrderProcessingSaga saga, CancellationToken cancellationToken);
    Task UpdateAsync(OrderProcessingSaga orderProcessing, CancellationToken cancellationToken);
}