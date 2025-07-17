using PixelzOrderSystem.Shared.Domain.Base;
using PixelzOrderSystem.Shared.Domain.Events;
using PixelzOrderSystem.Shared.Enums;

namespace PixelzOrderSystem.Shared.Domain.Entities;

public class Order: AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Amount { get; private set; }
    public OrderStatus Status { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CheckedOutAt { get; private set; }
    public string? PaymentTransactionId { get; private set; }
    
    /// <summary>
    /// Dùng cho EF Core
    /// </summary>
    private Order(){}
    
    public Order(string name, string description, decimal amount, Guid customerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Amount = amount;
        CustomerId = customerId;
        Status = OrderStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void CheckOut(string paymentTransactionId)
    {
        if (Status != OrderStatus.Created)
        {
            throw new InvalidOperationException("Order can only be checked out if it is in Created status.");
        }

        Status = OrderStatus.CheckedOut;
        CheckedOutAt = DateTime.UtcNow;
        PaymentTransactionId = paymentTransactionId;
        
        AddDomainEvent(new OrderCheckedOutEvent(
            Id, Name, Amount, CustomerId, paymentTransactionId, CheckedOutAt.Value));
    }
    
    public void MarkAsInProduction()
    {
        if (Status != OrderStatus.CheckedOut)
            throw new InvalidOperationException("Order must be checked out first");

        Status = OrderStatus.InProduction;
    }
}