using MediatR;
using PixelzOrderSystem.Shared.Domain.Events;
using PixelzOrderSystem.Shared.Dto.Invoices;
using PixelzOrderSystem.Shared.Repositories.Customers;
using PixelzOrderSystem.Shared.Repositories.OrderProcessing;
using PixelzOrderSystem.Shared.Services.Invoices;

namespace PixelzOrderSystem.Features.EventHandlers.OrderCheckedOut;

public class CreateInvoiceHandler(
    IInvoiceService invoiceService,
    IOrderProcessingRepository orderProcessingRepository,
    IMediator mediator,
    ILogger<CreateInvoiceHandler> logger
    ): INotificationHandler<CreateInvoiceStepCommand>
{
    public async Task Handle(CreateInvoiceStepCommand notification, CancellationToken cancellationToken)
    {
        var orderProcessing = await orderProcessingRepository.GetByIdAsync(notification.SagaId, cancellationToken);
        if (orderProcessing == null)
        {
            logger.LogError("Order processing saga with ID {SagaId} not found", notification.SagaId);
            return;
        }

        orderProcessing.CurrentStep = "GenerateInvoice";

        var step = orderProcessing.Steps.First(s => s.Name == "GenerateInvoice");

        logger.
            LogInformation("Processing {Step} for OrderProcessing with ID {OrderProcessingId}",step.Name,
                orderProcessing.Id);
           try
        {
            var orderCheckedOutEvent = notification.OrderEvent;

            var createInvoiceResult = await invoiceService.CreateInvoiceAsync(new CreateInvoiceRequest
            {
                Amount = notification.OrderEvent.Amount,
                OrderId = orderCheckedOutEvent.OrderId,
            });

            if (!createInvoiceResult.IsSuccess)
            {
                logger.LogError("Failed to create invoice for Order ID {OrderId}: {ErrorMessage}",
                    orderCheckedOutEvent.OrderId, createInvoiceResult.Message);
                step.Status = Shared.Enums.StepStatus.Failed;
                orderProcessing.Status = Shared.Enums.OrderProcessing.Failed;
                orderProcessing.FailedAt = DateTime.UtcNow;
                orderProcessing.ErrorMessage = createInvoiceResult.Message;
                await orderProcessingRepository.UpdateAsync(orderProcessing, cancellationToken);
                return;
            }

            step.Status = Shared.Enums.StepStatus.Completed;
            await orderProcessingRepository.UpdateAsync(orderProcessing, cancellationToken);

            await mediator.Publish(
                new PushToProductionStepCommand(orderProcessing.Id, orderCheckedOutEvent), cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to generate invoice for order {OrderId}", notification.OrderEvent.OrderId);
            step.Status = Shared.Enums.StepStatus.Failed;
            orderProcessing.Status = Shared.Enums.OrderProcessing.Failed;
            orderProcessing.FailedAt = DateTime.UtcNow;
            orderProcessing.ErrorMessage = e.Message;
            await orderProcessingRepository.UpdateAsync(orderProcessing, cancellationToken);
        }
    }
}