namespace PixelzOrderSystem.Shared.Dto.Invoices;

public class CreateInvoiceResult
{
    /// <summary>
    /// Trang thái của việc tạo hóa đơn.
    /// </summary>
    public bool IsSuccess { get; set; }
    /// <summary>
    /// ID của hóa đơn được tạo ra, nếu thành công.
    /// </summary>
    public string? InvoiceId { get; set; }
    /// <summary>
    /// Số hóa đơn.
    /// </summary>
    public string? InvoiceNumber { get; set; }
    /// <summary>
    /// Thông điệp trả về
    /// </summary>
    public string? Message { get; set; }
}