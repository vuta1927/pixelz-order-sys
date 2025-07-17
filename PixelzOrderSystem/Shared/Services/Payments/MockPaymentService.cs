using PixelzOrderSystem.Shared.Dto.Payments;

namespace PixelzOrderSystem.Shared.Services.Payments;

public class MockPaymentService(ILogger<MockPaymentService> logger): IPaymentService
{
    private readonly Random _random = new();
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            // Giả lập xử lý thanh toán
            await Task.Delay(1000);

            //sinh một số thực ngẫu nhiên từ 0 đến 1, nếu số này lớn hơn 0.1 (tức là 90% trường hợp), thì thanh toán thành công
            var isSuccess = _random.NextDouble() > 0.1;

            if (isSuccess)
            {
                var transactionId = Guid.NewGuid().ToString();
                logger.LogInformation("Order processed successfully. Order Id={OrderId}, Transaction: {TransactionId}",
                    request.OrderId, transactionId);

                return new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = transactionId
                };
            }

            logger.LogWarning("Order processing failed. Order Id={OrderId}", request.OrderId);
            return new PaymentResult
            {
                IsSuccess = false,
                Message = "Payment processing failed"
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error processing payment for order {OrderId}", request.OrderId);
            return new PaymentResult
            {
                IsSuccess = false,
                Message = "An error occurred while processing the payment"
            };
        }
    }
}