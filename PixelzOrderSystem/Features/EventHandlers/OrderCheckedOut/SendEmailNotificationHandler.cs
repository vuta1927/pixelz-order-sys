using MediatR;
using PixelzOrderSystem.Shared.Repositories.Customers;
using PixelzOrderSystem.Shared.Repositories.OrderProcessing;
using PixelzOrderSystem.Shared.Services.Emails;

namespace PixelzOrderSystem.Features.EventHandlers.OrderCheckedOut;

public class SendEmailNotificationHandler(
    IEmailService emailService,
    IOrderProcessingRepository orderProcessingRepository,
    ICustomerRepository customerRepository,
    IMediator mediator,
    ILogger<SendEmailNotificationHandler> logger):INotificationHandler<SendEmailStepCommand>
{
    public async Task Handle(SendEmailStepCommand notification, CancellationToken cancellationToken)
    {
        var orderProcessing = await orderProcessingRepository.GetByIdAsync(notification.SagaId, cancellationToken);
        if (orderProcessing == null)
        {
            logger.LogError("Order processing saga with ID {SagaId} not found", notification.SagaId);
            return;
        }
        orderProcessing.CurrentStep = "GenerateInvoice";

        var step = orderProcessing.Steps.First(s => s.Name == "SendEmail");

        logger.
            LogInformation("Processing {Step} for OrderProcessing with ID {OrderProcessingId}",step.Name,
            orderProcessing.Id);
        try
        {
            var orderCheckedOutEvent = notification.OrderEvent;

            var customer = await customerRepository.GetCustomerByIdAsync(orderCheckedOutEvent.CustomerId);
            if (customer == null)
            {
                // Handle the case where the customer is not found
                logger.LogError("Customer with ID {CustomerId} not found", orderCheckedOutEvent.CustomerId);
                step.Status = Shared.Enums.StepStatus.Failed;
                orderProcessing.Status = Shared.Enums.OrderProcessing.Failed;
                orderProcessing.FailedAt = DateTime.UtcNow;
                orderProcessing.ErrorMessage = $"Customer ID={orderCheckedOutEvent.CustomerId} not found";
                await orderProcessingRepository.UpdateAsync(orderProcessing, cancellationToken);
                return;
            }

            var orderId = orderCheckedOutEvent.OrderId;
            var subject = "Order Confirmation";
            var body = $"<span> Your order with ID <b>{orderId}<b> has been successfully checked out.</span>";

            await emailService.SendAsync(customer.Email, subject, body);

            step.Status = Shared.Enums.StepStatus.Completed;
            await orderProcessingRepository.UpdateAsync(orderProcessing, cancellationToken);

            await mediator.Publish(
                new CreateInvoiceStepCommand(orderProcessing.Id, orderCheckedOutEvent), cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send email notification for order {OrderId}", notification.OrderEvent.OrderId);
            step.Status = Shared.Enums.StepStatus.Failed;
            orderProcessing.Status = Shared.Enums.OrderProcessing.Failed;
            orderProcessing.FailedAt = DateTime.UtcNow;
            orderProcessing.ErrorMessage = e.Message;
            await orderProcessingRepository.UpdateAsync(orderProcessing, cancellationToken);
        }
    }
}