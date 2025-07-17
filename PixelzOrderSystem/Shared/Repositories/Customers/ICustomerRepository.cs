using PixelzOrderSystem.Shared.Domain.Entities;

namespace PixelzOrderSystem.Shared.Repositories.Customers;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(Guid customerId, bool tracking = false);
}