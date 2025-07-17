using FastEndpoints;
using MediatR;

namespace PixelzOrderSystem.Features.CheckoutOrder;

public class CheckoutOrderEndpoint(ISender sender): Endpoint<CheckoutOrderCommand, CheckoutOrderResponse>
{
    public override void Configure()
    {
        Post("/orders/checkout");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Thanh toán đơn hàng";
            s.Description = "API này dùng để thanh toán đơn hàng.";
            s.Response<CheckoutOrderResponse>(200, "Đơn hàng đã được thanh toán thành công");
        });
    }

    public override async Task HandleAsync(CheckoutOrderCommand request, CancellationToken ct)
    {
        var response = await sender.Send(request, ct);
        await SendAsync(response, cancellation: ct);
    }
}