using PixelzOrderSystem.Shared.Dto.Payments;

namespace PixelzOrderSystem.Shared.Services.Payments;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}