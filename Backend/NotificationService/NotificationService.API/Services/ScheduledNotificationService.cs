using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Constants;
using NotificationService.Application.Notifications.DTOs;
using NotificationService.Infrastructure.Persistence.DBContext;

namespace NotificationService.API.Services;

public class ScheduledNotificationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduledNotificationService> _logger;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(1);

    public ScheduledNotificationService(
        IServiceScopeFactory scopeFactory,
        ILogger<ScheduledNotificationService> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScheduledNotificationService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessScheduledNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ScheduledNotificationService");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    private async Task ProcessScheduledNotificationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hubService = scope.ServiceProvider.GetRequiredService<INotificationHubService>();

        var now = DateTime.UtcNow;

        var dueNotifications = await context
            .Notifications.Where(n =>
                n.Status == NotificationConstants.NotificationStatus.PENDING
                && n.ScheduledFor.HasValue
                && n.ScheduledFor.Value <= now
                && !n.IsReminderSent
                && n.IsActive
            )
            .ToListAsync(cancellationToken);

        foreach (var notification in dueNotifications)
        {
            notification.Status = NotificationConstants.NotificationStatus.SENT;
            notification.SentAt = now;
            notification.IsReminderSent = true;
            notification.UpdatedAt = now;
        }

        if (dueNotifications.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        foreach (var notification in dueNotifications)
        {
            try
            {
                await hubService.PushToUserAsync(
                    notification.UserId.ToString(),
                    NotificationDto.FromEntity(notification),
                    cancellationToken
                );

                _logger.LogInformation(
                    "Scheduled notification {Id} sent to user {UserId}",
                    notification.Id,
                    notification.UserId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to push scheduled notification {Id}", notification.Id);
            }
        }
    }
}
