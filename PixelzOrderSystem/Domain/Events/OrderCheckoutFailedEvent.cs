using System;
using MediatR;

namespace PixelzOrderSystem.Domain.Events;

public record OrderCheckoutFailedEvent(
    Guid OrderId,
    string Reason,
    DateTime FailedAt
) : INotification;