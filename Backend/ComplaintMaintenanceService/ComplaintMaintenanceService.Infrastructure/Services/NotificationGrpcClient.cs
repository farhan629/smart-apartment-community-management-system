using ComplaintMaintenanceService.Application.Interfaces.Services;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationService.API.Grpc;

namespace ComplaintMaintenanceService.Infrastructure.Services;

/// <summary>
/// gRPC client implementation for calling NotificationService.
/// </summary>
public class NotificationGrpcClient : INotificationGrpcClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationGrpcClient> _logger;

    public NotificationGrpcClient(
        IConfiguration configuration,
        ILogger<NotificationGrpcClient> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PushNotificationAsync(
        Guid userId,
        string notificationType,
        string title,
        string message,
        Guid complaintId,
        string? recipientEmail = null,
        string? recipientName = null,
        CancellationToken ct = default
    )
    {
        var url =
            _configuration[CmsGrpcConfigKeys.NotificationServiceUrl]
            ?? throw new InvalidOperationException(CmsGrpcConfigKeys.NotificationServiceUrlMissing);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new NotificationGrpc.NotificationGrpcClient(channel);

        var templateResponse = await client.GetTemplateIdByTypeAsync(
            new GetTemplateIdRequest { NotificationType = notificationType },
            cancellationToken: ct
        );

        if (!templateResponse.Found)
        {
            _logger.LogWarning(
                "NotificationGrpcClient - no template for type {Type}, skipping notification for user {UserId}",
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
            ComplaintId = complaintId.ToString(),
            RecipientEmail = recipientEmail ?? string.Empty,
            RecipientName = recipientName ?? string.Empty,
        };

        var result = await client.PushNotificationAsync(request, cancellationToken: ct);

        _logger.LogInformation(
            "NotificationGrpcClient - pushed {Type} to user {UserId}, success={Success}, notificationId={Id}",
            notificationType,
            userId,
            result.Success,
            result.NotificationId
        );
    }

    public async Task<Guid?> GetTemplateIdByTypeAsync(
        string notificationType,
        CancellationToken ct = default
    )
    {
        var url =
            _configuration[CmsGrpcConfigKeys.NotificationServiceUrl]
            ?? throw new InvalidOperationException(CmsGrpcConfigKeys.NotificationServiceUrlMissing);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new NotificationGrpc.NotificationGrpcClient(channel);

        var response = await client.GetTemplateIdByTypeAsync(
            new GetTemplateIdRequest { NotificationType = notificationType },
            cancellationToken: ct
        );

        return response.Found && Guid.TryParse(response.TemplateId, out var id) ? id : null;
    }
}
