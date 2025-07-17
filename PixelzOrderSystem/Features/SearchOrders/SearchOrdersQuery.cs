using MediatR;

namespace PixelzOrderSystem.Features.SearchOrders;

public record SearchOrdersQuery(string? Name = null) : IRequest<IEnumerable<OrderDto>>;

public record OrderDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Amount,
    string Status,
    DateTime CreatedAt,
    DateTime? CheckedOutAt
);