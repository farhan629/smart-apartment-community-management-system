using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Constants;

namespace NotificationService.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId =
            Context.User?.FindFirst(NotificationConstants.JwtClaims.USER_ID)?.Value
            ?? Context.User?.FindFirst(NotificationConstants.JwtClaims.SUB)?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            _logger.LogInformation(
                "User {UserId} connected to NotificationHub — connectionId: {ConnectionId}",
                userId,
                Context.ConnectionId
            );
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId =
            Context.User?.FindFirst(NotificationConstants.JwtClaims.USER_ID)?.Value
            ?? Context.User?.FindFirst(NotificationConstants.JwtClaims.SUB)?.Value;

        _logger.LogInformation(
            "User {UserId} disconnected — connectionId: {ConnectionId}",
            userId,
            Context.ConnectionId
        );

        await base.OnDisconnectedAsync(exception);
    }
}
