using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Constants;
using NotificationService.Application.Notifications.Commands.SendEmail;
using NotificationService.Application.Notifications.DTOs;
using NotificationService.Domain.Entities;
using Shared.SharedLibrary.Exceptions;

namespace NotificationService.Application.Notifications.Commands.PushNotification;

/// <summary>
/// Command that creates a new in-app notification for a user and optionally triggers
/// a transactional email via <see cref="SendEmailCommand"/>.
/// </summary>
public class PushNotificationCommand : IRequest<NotificationDto>
{
    public Guid UserId { get; }
    public Guid TemplateId { get; }
    public string Title { get; }
    public string Message { get; }
    public string NotificationType { get; }
    public Guid? VisitId { get; }
    public Guid? ComplaintId { get; }
    public Guid? AmenityBookingId { get; }
    public string? RecipientEmail { get; }
    public string? RecipientName { get; }
    public DateTime? ScheduledFor { get; }

    public PushNotificationCommand(
        Guid userId,
        Guid templateId,
        string title,
        string message,
        string notificationType,
        Guid? visitId = null,
        Guid? complaintId = null,
        Guid? amenityBookingId = null,
        string? recipientEmail = null,
        string? recipientName = null,
        DateTime? scheduledFor = null
    )
    {
        UserId = userId;
        TemplateId = templateId;
        Title = title;
        Message = message;
        NotificationType = notificationType;
        VisitId = visitId;
        ComplaintId = complaintId;
        AmenityBookingId = amenityBookingId;
        RecipientEmail = recipientEmail;
        RecipientName = recipientName;
        ScheduledFor = scheduledFor;
    }
}

/// <summary>
/// Handles <see cref="PushNotificationCommand"/> by persisting the in-app notification
/// and conditionally dispatching email delivery.
/// </summary>
public class PushNotificationCommandHandler
    : IRequestHandler<PushNotificationCommand, NotificationDto>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMediator _mediator;
    private readonly INotificationHubService _hubService;
    private readonly ILogger<PushNotificationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="PushNotificationCommandHandler"/>.
    /// </summary>
    public PushNotificationCommandHandler(
        INotificationRepository notificationRepository,
        IMediator mediator,
        INotificationHubService hubService,
        ILogger<PushNotificationCommandHandler> logger
    )
    {
        _notificationRepository = notificationRepository;
        _mediator = mediator;
        _hubService = hubService;
        _logger = logger;
    }

    /// <summary>
    /// Validates the command, builds and persists the <see cref="Notification"/> entity,
    /// optionally dispatches email delivery, and pushes a real-time SignalR update to the user.
    /// </summary>
    public async Task<NotificationDto> Handle(
        PushNotificationCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
            throw new BadRequestException(NotificationConstants.Errors.TITLE_AND_MESSAGE_REQUIRED);

        var isScheduled =
            request.ScheduledFor.HasValue && request.ScheduledFor.Value > DateTime.UtcNow;

        var notification = new Notification
        {
            UserId = request.UserId,
            TemplateId = request.TemplateId,
            Title = request.Title,
            Message = request.Message,
            NotificationType = string.IsNullOrWhiteSpace(request.NotificationType)
                ? NotificationConstants.DEFAULT_NOTIFICATION_TYPE
                : request.NotificationType.ToLowerInvariant(),
            VisitId = request.VisitId,
            ComplaintId = request.ComplaintId,
            AmenityBookingId = request.AmenityBookingId,
            IsRead = false,
            IsActive = true,
            Status = isScheduled
                ? NotificationConstants.NotificationStatus.PENDING
                : NotificationConstants.NotificationStatus.SENT,
            SentAt = isScheduled ? null : DateTime.UtcNow,
            ScheduledFor = isScheduled ? request.ScheduledFor : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var saved = await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Notification saved - id: {Id}, user: {UserId}, type: {Type}, status: {Status}, scheduledFor: {ScheduledFor}",
            saved.Id,
            request.UserId,
            request.NotificationType,
            saved.Status,
            saved.ScheduledFor?.ToString("o") ?? "null"
        );

        if (!string.IsNullOrWhiteSpace(request.RecipientEmail))
        {
            try
            {
                await _mediator.Send(
                    new SendEmailCommand(
                        userId: request.UserId,
                        notificationType: request.NotificationType,
                        recipientEmail: request.RecipientEmail,
                        recipientName: request.RecipientName ?? string.Empty,
                        placeholders: new Dictionary<string, string>
                        {
                            ["title"] = request.Title,
                            ["message"] = request.Message,
                        }
                    ),
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Email dispatch failed for user {UserId} - notification already saved",
                    request.UserId
                );
            }
        }

        if (!isScheduled)
        {
            await _hubService.PushToUserAsync(
                request.UserId.ToString(),
                NotificationDto.FromEntity(saved),
                cancellationToken
            );
        }

        return NotificationDto.FromEntity(saved);
    }
}
