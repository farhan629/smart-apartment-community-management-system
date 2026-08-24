namespace ResidentVisitorService.Application.Interfaces.Services;

public interface INotificationClient
{
    Task NotifyAsync(
        Guid userId,
        string notificationType,
        string title,
        string message,
        Guid? visitId = null,
        string? recipientEmail = null,
        string? recipientName = null,
        string? qrCodeUrl = null,
        string? visitDate = null,
        CancellationToken cancellationToken = default
    );
}
