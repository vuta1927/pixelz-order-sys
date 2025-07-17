using PixelzOrderSystem.Shared.Enums;

namespace PixelzOrderSystem.Shared.Domain.Entities;

public class DomainEvent
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; }
    public string? LastError { get; set; }
    public DomainEventStatus Status { get; set; }
}