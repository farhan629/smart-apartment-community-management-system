using Grpc.Core;
using Microsoft.Extensions.Logging;
using NotificationService.API.Grpc;
using ResidentVisitorService.Application.Interfaces.Services;

namespace ResidentVisitorService.Infrastructure.Services;

/// <summary>
/// gRPC-based implementation of <see cref="INotificationClient"/>.
/// Resolves the notification template ID dynamically from NotificationService
/// before dispatching the push-notification call.
/// </summary>
public class NotificationClient : INotificationClient
{
    private readonly NotificationGrpc.NotificationGrpcClient _grpcClient;
    private readonly ILogger<NotificationClient> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="NotificationClient"/>.
    /// </summary>
    public NotificationClient(
        NotificationGrpc.NotificationGrpcClient grpcClient,
        ILogger<NotificationClient> logger
    )
    {
        _grpcClient = grpcClient;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task NotifyAsync(
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
    )
    {
        try
        {
            var templateResponse = await _grpcClient.GetTemplateIdByTypeAsync(
                new GetTemplateIdRequest { NotificationType = notificationType },
                cancellationToken: cancellationToken
            );

            if (!templateResponse.Found)
            {
                _logger.LogWarning(
                    "No notification template found for type {Type} — skipping notification for user {UserId}",
                    notificationType,
                    userId
                );
                return;
            }

            var request = new PushNotificationRequest
            {
                UserId = userId.ToString(),
                TemplateId = templateResponse.TemplateId,
                Title = title,
                Message = message,
                NotificationType = notificationType,
                VisitId = visitId?.ToString() ?? string.Empty,
                RecipientEmail = recipientEmail ?? string.Empty,
                RecipientName = recipientName ?? string.Empty,
                QrCodeUrl = qrCodeUrl ?? string.Empty,
                VisitDate = visitDate ?? string.Empty,
            };
            await _grpcClient.PushNotificationAsync(request, cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Notification sent — type: {Type}, user: {UserId}",
                notificationType,
                userId
            );
        }
        catch (RpcException ex)
        {
            _logger.LogError(
                ex,
                "gRPC error sending notification — type: {Type}, user: {UserId}, status: {Status}",
                notificationType,
                userId,
                ex.StatusCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send notification — type: {Type}, user: {UserId}",
                notificationType,
                userId
            );
        }
    }
}
