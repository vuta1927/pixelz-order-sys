using MediatR;
using PixelzOrderSystem.Shared;
using PixelzOrderSystem.Shared.Dto.Payments;
using PixelzOrderSystem.Shared.Repositories.Customers;
using PixelzOrderSystem.Shared.Repositories.Orders;
using PixelzOrderSystem.Shared.Services.Payments;

namespace PixelzOrderSystem.Features.CheckoutOrder;

public class CheckoutOrderHandler(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IPaymentService paymentService,
    ILogger<CheckoutOrderHandler> logger): IRequestHandler<CheckoutOrderCommand, CheckoutOrderResponse>
{
    public async Task<CheckoutOrderResponse> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                return new CheckoutOrderResponse(
                    false, "Không tìm thấy đơn hàng. Vui lòng kiểm tra lại thông tin và thử lại."
                );
            }

            var customer = await customerRepository.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null)
            {
                return new CheckoutOrderResponse(
                    false, "Không tìm thấy khách hàng. Vui lòng kiểm tra lại thông tin và thử lại."
                );
            }

            // Process payment
            var paymentResult = await paymentService.ProcessPaymentAsync(new PaymentRequest
            {
                OrderId = order.Id.ToString(),
                Content = Constant.PaymentContentTemplate
                    .Replace("{0}", order.Name)
                    .Replace("{1}", order.Id.ToString()),
                Amount = order.Amount,
                PaymentType = request.PaymentType
            });
            if (!paymentResult.IsSuccess)
            {
                logger.LogWarning("Payment failed for Customer ID {CustomerId}: {ErrorMessage}",
                    customer.Id, paymentResult.Message);
                return new CheckoutOrderResponse(
                    false, $"Thanh toán không thành công: {paymentResult.Message}"
                );
            }

            order.CheckOut(paymentResult.TransactionId);
            await orderRepository.UpdateAsync(order);

            return new CheckoutOrderResponse(true, "Đơn hàng đã được thanh toán thành công và trong quá trình xử lý.", paymentResult.TransactionId);

        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while processing the checkout order command");
            return new CheckoutOrderResponse(
                false, "Đã xảy ra lỗi trong quá trình xử lý đơn hàng. Vui lòng thử lại sau."
            );
        }
    }
}