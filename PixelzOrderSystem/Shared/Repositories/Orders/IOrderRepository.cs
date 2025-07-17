using PixelzOrderSystem.Shared.Domain.Entities;

namespace PixelzOrderSystem.Shared.Repositories.Orders;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> SearchAsync(string? name = null);
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> UpdateAsync(Order order);
}