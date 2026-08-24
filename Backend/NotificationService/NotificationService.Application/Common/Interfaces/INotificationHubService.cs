namespace NotificationService.Application.Common.Interfaces;

/// <summary>
/// Abstracts the SignalR hub push so the Application layer never
/// directly references Microsoft.AspNetCore.SignalR.
/// Clean Architecture: Application defines the contract, Infrastructure implements it.
/// </summary>
public interface INotificationHubService
{
    Task PushToUserAsync(
        string userId,
        object payload,
        CancellationToken cancellationToken = default
    );
}
