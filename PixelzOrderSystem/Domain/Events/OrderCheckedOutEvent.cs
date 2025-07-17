using System;
using MediatR;

namespace PixelzOrderSystem.Domain.Events;

public record OrderCheckedOutEvent(
    Guid OrderId,
    string OrderName,
    decimal Amount,
    Guid CustomerId,
    string PaymentTransactionId,
    DateTime CheckedOutAt
) : INotification;