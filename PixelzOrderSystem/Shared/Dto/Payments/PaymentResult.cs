namespace PixelzOrderSystem.Shared.Dto.Payments;

public class PaymentResult
{
    /// <summary>
    /// Kết quả thanh toán
    /// </summary>
    public bool IsSuccess { get; set; }
    /// <summary>
    /// Mã giao dịch
    /// </summary>
    public string TransactionId { get; set; }
    /// <summary>
    /// Thông điệp trả về từ cổng thanh toán
    /// </summary>
    public string? Message { get; set; }
}