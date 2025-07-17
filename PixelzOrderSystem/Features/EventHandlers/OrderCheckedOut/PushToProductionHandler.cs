using MediatR;
using PixelzOrderSystem.Shared.Dto.Production;
using PixelzOrderSystem.Shared.Repositories.OrderProcessing;
using PixelzOrderSystem.Shared.Services.Production;

namespace PixelzOrderSystem.Features.EventHandlers.OrderCheckedOut;

public class PushToProductionHandler(
    IProductionService productionService,
    IOrderProcessingRepository orderProcessingRepository,
    ILogger<CreateInvoiceHandler> logger
    ): INotificationHandler<PushToProductionStepCommand>
{
    public async Task Handle(PushToProductionStepCommand notification, CancellationToken cancellationToken)
    {
        var orderProcessing = await orderProcessingRepository.GetByIdAsync(notification.SagaId, cancellationToken);
        if (orderProcessing == null)
        {
            logger.LogError("Order processing saga with ID {SagaId} not found", notification.SagaId);
            return;
        }
        orderProcessing.CurrentStep = "PushToProduction";

        var step = orderProcessing.Steps.First(s => s.Name == "GenerateInvoice");

        logger.
            LogInformation("Processing {Step} for OrderProcessing with ID {OrderProcessingId}",step.Name,
                orderProcessing.Id);
           try
        {
            var orderCheckedOutEvent = notification.OrderEvent;

            var pushProductionResult = await productionService.PushOrderAsync(new ProductionOrderRequest
            {
                OrderId = orderCheckedOutEvent.OrderId,
                CustomerId = orderCheckedOutEvent.CustomerId,
                Amount = orderCheckedOutEvent.Amount,
                OrderName = orderCheckedOutEvent.OrderName,
                PaymentTransactionId = orderCheckedOutEvent.PaymentTransactionId
            });

            if (!pushProductionResult.IsSuccess)
            {
                logger.LogError("Failed to push production service for Order ID {OrderId}: {ErrorMessage}",
                    orderCheckedOutEvent.OrderId, pushProductionResult.Message);
                step.Status = Shared.Enums.StepStatus.Failed;
                orderProcessing.Status = Shared.Enums.OrderProcessing.Failed;
                orderProcessing.FailedAt = DateTime.UtcNow;
                orderProcessing.ErrorMessage = pushProductionResult.Message;
                await orderProcessingRepository.UpdateAsync(orderProcessing, cancellationToken);
                return;
            }

            step.Status = Shared.Enums.StepStatus.Completed;
            await orderProcessingRepository.UpdateAsync(orderProcessing, cancellationToken);
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