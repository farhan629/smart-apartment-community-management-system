namespace ComplaintMaintenanceService.Application.Interfaces.Services;

/// <summary>
/// Abstraction for calling NotificationService via gRPC.
/// </summary>
public interface INotificationGrpcClient
{
    /// <summary>
    /// Sends an in-app notification (and optionally email) to a user.
    /// Pass recipientEmail to trigger email; leave null for in-app only.
    /// </summary>
    Task PushNotificationAsync(
        Guid userId,
        string notificationType,
        string title,
        string message,
        Guid complaintId,
        string? recipientEmail = null,
        string? recipientName = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Resolves the notification template ID for a given notification type.
    /// Returns null if not found.
    /// </summary>
    Task<Guid?> GetTemplateIdByTypeAsync(string notificationType, CancellationToken ct = default);
}
