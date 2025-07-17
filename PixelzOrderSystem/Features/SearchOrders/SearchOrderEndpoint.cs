using FastEndpoints;
using MediatR;

namespace PixelzOrderSystem.Features.SearchOrders;

public class SearchOrderEndpoint(ISender sender): Endpoint<SearchOrdersQuery, IEnumerable<OrderDto>>
{
    public override void Configure()
    {
        Get("/orders");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Tìm kiếm đơn hàng";
            s.Description = "API này dùng để tìm kiếm đơn hàng theo tên.";
            s.Response<IEnumerable<OrderDto>>(200, "Danh sách đơn hàng");
        });

    }

    public override async Task HandleAsync(SearchOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await sender.Send(request, cancellationToken);
        await SendAsync(orders, cancellation: cancellationToken);
    }
}