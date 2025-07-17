namespace PixelzOrderSystem.Shared.Dto.Production;

public class ProductionOrderRequest
{
    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    public Guid OrderId { get; set; }
    /// <summary>
    /// Tên đơn hàng
    /// </summary>
    public string OrderName { get; set; }
    /// <summary>
    /// Số tiền thanh toán
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// Mã Khách hàng
    /// </summary>
    public Guid CustomerId { get; set; }
    /// <summary>
    /// Mã giao dịch thanh toán
    /// </summary>
    public string PaymentTransactionId { get; set; }
    /// <summary>
    /// Thời gian tt
    /// </summary>
    public DateTime CheckedOutAt { get; set; }
}