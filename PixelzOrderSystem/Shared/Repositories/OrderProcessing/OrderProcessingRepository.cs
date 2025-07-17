using Microsoft.EntityFrameworkCore;
using PixelzOrderSystem.Shared.Database;
using PixelzOrderSystem.Shared.Domain.Entities;

namespace PixelzOrderSystem.Shared.Repositories.OrderProcessing;

public class OrderProcessingRepository(AppDbContext context): IOrderProcessingRepository
{
    public Task<OrderProcessingSaga?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.OrderProcessingSagas
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task AddSagaAsync(OrderProcessingSaga saga, CancellationToken cancellationToken)
    {
        context.OrderProcessingSagas.Add(saga);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(OrderProcessingSaga orderProcessing, CancellationToken cancellationToken)
    {
        context.OrderProcessingSagas.Update(orderProcessing);
        return context.SaveChangesAsync(cancellationToken);
    }
}