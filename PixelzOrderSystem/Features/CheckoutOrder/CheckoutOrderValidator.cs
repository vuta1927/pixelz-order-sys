using FluentValidation;

namespace PixelzOrderSystem.Features.CheckoutOrder;

public class CheckoutOrderValidator: AbstractValidator<CheckoutOrderCommand>
{
    public CheckoutOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID là bắt buộc.");

        RuleFor(x => x.PaymentType)
            .NotEmpty().WithMessage("Loại thanh toán là bắt buộc.")
            .IsInEnum().WithMessage("Loại thanh toán không hợp lệ.");
    }
}