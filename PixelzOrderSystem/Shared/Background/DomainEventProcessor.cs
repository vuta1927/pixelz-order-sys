using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PixelzOrderSystem.Shared.Database;
using PixelzOrderSystem.Shared.Domain.Entities;
using PixelzOrderSystem.Shared.Enums;

namespace PixelzOrderSystem.Shared.Background;

public class DomainEventProcessor(
    IServiceProvider serviceProvider,
    ILogger<DomainEventProcessor> logger): BackgroundService
{
    private const int ProcessingIntervalMs = 1000;
    private const int MaxRetryCount = 3;
    private const int MaxBatchSize = 10;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Domain EventProcessor is starting");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingDomainEventsAsync(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred while processing domain events");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            finally
            {
                await Task.Delay(ProcessingIntervalMs, stoppingToken);
            }
        }
    }

    private async Task ProcessPendingDomainEventsAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var pendingEvents = await dbContext.DomainEvents
            .Where(e => e.Status == DomainEventStatus.Pending &&
                        e.RetryCount < MaxRetryCount)
            .OrderBy(x => x.CreatedAt)
            .Take(MaxBatchSize)
            .ToListAsync(stoppingToken);

        if (!pendingEvents.Any())
            return;

        logger.LogInformation("Processing {Count} pending domain events", pendingEvents.Count);

        foreach (var pendingEvent in pendingEvents)
        {
            await ProcessSingleEvent(pendingEvent, dbContext, mediator, stoppingToken);
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }

    private async Task ProcessSingleEvent(DomainEvent eventToProcess, AppDbContext context, IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            eventToProcess.Status = DomainEventStatus.Processing;
            await context.SaveChangesAsync(cancellationToken);

            var entityType = Type.GetType(eventToProcess.Type);
            var domainEvent = entityType != null
                ? JsonSerializer.Deserialize(eventToProcess.Data, entityType)
                : null;

            if (domainEvent is INotification notification)
            {
                await mediator.Publish(notification, cancellationToken);
                eventToProcess.Status = DomainEventStatus.Processed;
                eventToProcess.ProcessedAt = DateTime.UtcNow;
                eventToProcess.LastError = null;

                logger.LogInformation("Successfully processed domain event {EventId} of type {EventType}",
                    eventToProcess
                        .Id, eventToProcess.Type);
            }
        }
        catch (Exception e)
        {
            eventToProcess.RetryCount++;
            eventToProcess.LastError = e.Message;

            if(eventToProcess.RetryCount >= MaxRetryCount)
            {
                eventToProcess.Status = DomainEventStatus.Failed;
                logger.LogError(e, "Failed to process domain event {EventId} after {RetryCount} retries",
                    eventToProcess.Id, eventToProcess.RetryCount);
            }
            else
            {
                eventToProcess.Status = DomainEventStatus.Pending;
                logger.LogWarning(e, "Retrying domain event {EventId}, attempt {RetryCount}",
                    eventToProcess.Id, eventToProcess.RetryCount);
            }
        }
    }
}