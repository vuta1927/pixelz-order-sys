using PixelzOrderSystem.Shared.Enums;

namespace PixelzOrderSystem.Shared.Dto.Payments;

public class PaymentRequest
{
    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    public string OrderId { get; set; }
    /// <summary>
    /// Số tiền thanh toán
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// Nội dung thanh toán
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// Loại thanh toán
    /// </summary>
    public PaymentType PaymentType { get; set; }
}