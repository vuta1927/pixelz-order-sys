using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PixelzOrderSystem.Shared.Domain.Base;
using PixelzOrderSystem.Shared.Domain.Entities;
using PixelzOrderSystem.Shared.Enums;

namespace PixelzOrderSystem.Shared.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// Dùng để lưu trữ các domain events, các events sẽ được xử lý bởi Job
    /// </summary>
    public DbSet<DomainEvent> DomainEvents { get; set; }

    /// <summary>
    /// Dùng để quản lý các bước trong quy trình xử lý đơn hàng
    /// Ví dụ: Gửi email, tạo hoá đơn, gửi đến production service
    /// Mỗi bước sẽ có trạng thái và thông tin lỗi nếu có
    /// </summary>
    public DbSet<OrderProcessingSaga> OrderProcessingSagas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Name);
            entity.Ignore(e => e.DomainEvents); // Bỏ qua DomainEvents
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Lấy tất cả domain events từ entity thuộc type AggregateRoot
            var domainEvents = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents.Any())
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // Lưu các domain events vào bảng DomainEvents để xử lý sau này
            foreach (var domainEvent in domainEvents)
            {
                var @event = new DomainEvent
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().FullName,
                    Data = JsonConvert.SerializeObject(domainEvent),
                    CreatedAt = DateTime.UtcNow,
                    Status = DomainEventStatus.Pending,
                    RetryCount = 0
                };
                DomainEvents.Add(@event);
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            // Xóa tất cả domain events sau khi đã publish, tránh việc insert event lại nếu SaveChanges được gọi lại
            foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
            {
                entry.Entity.ClearDomainEvents();
            }

            logger.LogInformation("Saved {EventCount} domain events to DB", domainEvents.Count);

            return result;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(e, "An error occurred while saving changes to the database");
            throw;
        }
    }
}