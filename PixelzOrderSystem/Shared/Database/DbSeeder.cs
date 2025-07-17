using Microsoft.EntityFrameworkCore;
using PixelzOrderSystem.Shared.Domain.Entities;

namespace PixelzOrderSystem.Shared.Database;

public static class DbSeeder
{
    public static async Task SeedSampleDataAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();
        await SeedCustomersAsync(context);
        await SeedOrdersAsync(context);
    }

    private static async Task SeedCustomersAsync(AppDbContext context)
    {
        if (await context.Customers.AnyAsync())
        {
            return;
        }
        var customers = new List<Customer>
        {
            new("John Doe", "abc@local.xxx"),
            new("Jane Smith", "jane@local.xxx")
        };
        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();
    }

    private static async Task SeedOrdersAsync(AppDbContext context)
    {
        var customer = await context.Customers.FirstOrDefaultAsync();
        if (customer == null)
        {
            throw new InvalidOperationException("No customers found to associate with orders.");
        }
        if (await context.Orders.AnyAsync())
        {
            return;
        }
        var orders = new List<Order>
        {
            new("Order 1", "Description for Order 1", 10000, customer.Id),
            new("Order 2", "Description for Order 2", 20000, customer.Id),
            new("Order 3", "Description for Order 3", 30000, customer.Id),
            new("Order 4", "Description for Order 4", 40000, customer.Id)
        };

        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();
    }
}