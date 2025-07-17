using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PixelzOrderSystem.Domain.Entities;

namespace PixelzOrderSystem.Infrastructure.Repositories.Orders;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> SearchAsync(string? name = null);
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> UpdateAsync(Order order);
}