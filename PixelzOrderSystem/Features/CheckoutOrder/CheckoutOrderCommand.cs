using MediatR;
using PixelzOrderSystem.Shared.Enums;

namespace PixelzOrderSystem.Features.CheckoutOrder;

public record CheckoutOrderCommand(Guid OrderId, PaymentType PaymentType) : IRequest<CheckoutOrderResponse>;

public record CheckoutOrderResponse(
    bool Success,
    string Message,
    string? TransactionId = null
);