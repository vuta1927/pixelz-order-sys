namespace PixelzOrderSystem.Shared.Dto.Production;

public class ProductionResult
{
    /// <summary>
    /// Kết quả xử lý đơn hàng
    /// </summary>
    public bool IsSuccess { get; set; }
    /// <summary>
    /// Thông điệp trả về
    /// </summary>
    public string? Message { get; set; }
}