using MediatR;
using PixelzOrderSystem.Shared.Repositories.Orders;

namespace PixelzOrderSystem.Features.SearchOrders;

public class SearchOrdersHandler(IOrderRepository orderRepository): IRequestHandler<SearchOrdersQuery, IEnumerable<OrderDto>>
{
    public async Task<IEnumerable<OrderDto>> Handle(SearchOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.SearchAsync(request.Name);

        return orders.Select(order => new OrderDto(
            order.Id,
            order.Name,
            order.Description,
            order.Amount,
            order.Status.ToString(),
            order.CreatedAt,
            order.CheckedOutAt
        ));
    }
}