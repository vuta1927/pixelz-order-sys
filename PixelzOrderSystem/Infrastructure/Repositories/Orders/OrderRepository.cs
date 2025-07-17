using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PixelzOrderSystem.Domain.Entities;
using PixelzOrderSystem.Infrastructure.Database;

namespace PixelzOrderSystem.Infrastructure.Repositories.Orders;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<IEnumerable<Order>> SearchAsync(string? name = null)
    {
        IQueryable<Order> query = context.Orders.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(o => o.Name.Contains(name));
        }

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await context.Orders.FindAsync(id);
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync();
        return order;
    }
}