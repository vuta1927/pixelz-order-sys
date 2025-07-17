using MediatR;
using Microsoft.EntityFrameworkCore;
using PixelzOrderSystem.Domain.Base;
using PixelzOrderSystem.Domain.Entities;

namespace PixelzOrderSystem.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    
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
        // Lấy tất cả domain events từ entity thuộc type AggregateRoot
        var domainEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Publish domain events
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        // Xóa tất cả domain events sau khi đã publish, tránh việc publish lại nếu SaveChanges được gọi lại
        foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return result;
    }
}