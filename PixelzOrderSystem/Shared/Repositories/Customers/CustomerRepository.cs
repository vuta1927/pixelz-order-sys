using Microsoft.EntityFrameworkCore;
using PixelzOrderSystem.Shared.Database;
using PixelzOrderSystem.Shared.Domain.Entities;

namespace PixelzOrderSystem.Shared.Repositories.Customers;

public class CustomerRepository(AppDbContext context): ICustomerRepository
{
    public Task<Customer?> GetCustomerByIdAsync(Guid customerId, bool tracking = false)
    {
        if (tracking)
        {
            return context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        return context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }
}