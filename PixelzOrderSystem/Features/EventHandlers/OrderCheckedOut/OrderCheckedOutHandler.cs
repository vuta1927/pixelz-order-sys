using MediatR;
using PixelzOrderSystem.Shared.Domain.Entities;
using PixelzOrderSystem.Shared.Domain.Events;
using PixelzOrderSystem.Shared.Repositories.OrderProcessing;

namespace PixelzOrderSystem.Features.EventHandlers.OrderCheckedOut;

public class OrderCheckedOutHandler(
    IOrderProcessingRepository orderProcessingRepository,
    IMediator mediator):
    INotificationHandler<OrderCheckedOutEvent>
{
    public async Task Handle(OrderCheckedOutEvent notification, CancellationToken cancellationToken)
    {
        var saga = new OrderProcessingSaga
        {
            Id = Guid.NewGuid(),
            OrderId = notification.OrderId,
            CurrentStep = "SendEmail",
            CreatedAt = DateTime.UtcNow,
            Status = Shared.Enums.OrderProcessing.Started,
            Steps =
            [
                new SagaStep()
                {
                    Name = "SendEmail",
                    Status = Shared.Enums.StepStatus.Pending
                },
                new SagaStep()
                {
                    Name = "GenerateInvoice",
                    Status = Shared.Enums.StepStatus.Pending
                },

                new SagaStep
                {
                    Name = "PushToProduction",
                    Status = Shared.Enums.StepStatus.Pending
                }
            ]
        };

        // Lưu saga vào cơ sở dữ liệu để theo dõi tiến trình xử lý đơn hàng
        await orderProcessingRepository.AddSagaAsync(saga, cancellationToken);
        await mediator.Publish(new SendEmailStepCommand(saga.Id, notification), cancellationToken);
    }
}

public record SendEmailStepCommand(Guid SagaId, OrderCheckedOutEvent OrderEvent) : INotification;

public record CreateInvoiceStepCommand(Guid SagaId, OrderCheckedOutEvent OrderEvent) : INotification;

public record PushToProductionStepCommand(Guid SagaId, OrderCheckedOutEvent OrderEvent) : INotification;