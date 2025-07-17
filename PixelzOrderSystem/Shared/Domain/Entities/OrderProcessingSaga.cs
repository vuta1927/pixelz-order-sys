using PixelzOrderSystem.Shared.Enums;

namespace PixelzOrderSystem.Shared.Domain.Entities;

/// <summary>
/// Chứa thông tin về tiến trình xử lý đơn hàng.
/// </summary>
public class OrderProcessingSaga
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string CurrentStep { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public OrderProcessing Status { get; set; }
    public string? ErrorMessage { get; set; }
    public List<SagaStep> Steps { get; set; } = [];
}

public class SagaStep
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public StepStatus Status { get; set; }
}