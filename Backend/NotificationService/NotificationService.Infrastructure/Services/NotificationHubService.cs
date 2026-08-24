using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Infrastructure.Hubs;

/// <summary>
/// SignalR implementation of <see cref="INotificationHubService"/> that pushes
/// real-time notifications to connected clients via <see cref="NotificationHub"/>.
/// </summary>
public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationHubService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="NotificationHubService"/>.
    /// </summary>
    public NotificationHubService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationHubService> logger
    )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task PushToUserAsync(
        string userId,
        object payload,
        CancellationToken cancellationToken = default
    )
    {
        await _hubContext
            .Clients.Group(userId)
            .SendAsync("ReceiveNotification", payload, cancellationToken);

        _logger.LogInformation("SignalR push sent to user {UserId}", userId);
    }
}
