using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Constants;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Notifications.Commands.SendEmail;

/// <summary>
/// Command that carries all data required to send a single transactional email to a recipient.
/// </summary>
public class SendEmailCommand : IRequest
{
    public Guid UserId { get; }
    public string NotificationType { get; }
    public string RecipientEmail { get; }
    public string RecipientName { get; }
    public Dictionary<string, string>? Placeholders { get; }

    public SendEmailCommand(
        Guid userId,
        string notificationType,
        string recipientEmail,
        string recipientName,
        Dictionary<string, string>? placeholders = null
    )
    {
        UserId = userId;
        NotificationType = notificationType;
        RecipientEmail = recipientEmail;
        RecipientName = recipientName;
        Placeholders = placeholders;
    }
}

/// <summary>
/// Handles <see cref="SendEmailCommand"/> by resolving the email template, substituting placeholders,
/// dispatching the email, and persisting an <see cref="EmailLog"/> record regardless of outcome.
/// </summary>
public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand>
{
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<SendEmailCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="SendEmailCommandHandler"/>.
    /// </summary>
    public SendEmailCommandHandler(
        IEmailLogRepository emailLogRepository,
        IEmailService emailService,
        ILogger<SendEmailCommandHandler> logger
    )
    {
        _emailLogRepository = emailLogRepository;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the email send pipeline: template lookup → placeholder substitution →
    /// delivery → log persistence.
    /// </summary>
    public async Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        EmailTemplate? template = await _emailLogRepository.GetTemplateByTypeAsync(
            request.NotificationType
        );

        if (template is null)
        {
            _logger.LogWarning(
                "No active email template for type {Type} — skipping email for user {UserId}",
                request.NotificationType,
                request.UserId
            );

            return;
        }

        string subject = ReplacePlaceholders(template.Subject, request);
        string body = ReplacePlaceholders(template.BodyTemplate, request);

        var emailLog = new EmailLog
        {
            TemplateId = template.Id,
            UserId = request.UserId,
            EmailAddress = request.RecipientEmail,
            Subject = subject,
            Body = body,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        try
        {
            await _emailService.SendEmailAsync(
                request.RecipientEmail,
                request.RecipientName,
                subject,
                body
            );

            emailLog.Status = NotificationConstants.EmailLogStatus.SENT;
            emailLog.SentAt = DateTime.UtcNow;

            _logger.LogInformation(
                "Email sent to {Email} (type: {Type}, user: {UserId})",
                request.RecipientEmail,
                request.NotificationType,
                request.UserId
            );
        }
        catch (Exception ex)
        {
            emailLog.Status = NotificationConstants.EmailLogStatus.FAILED;
            emailLog.ErrorMessage = ex.Message;

            _logger.LogError(
                ex,
                "Failed to send email to {Email} (type: {Type}, user: {UserId})",
                request.RecipientEmail,
                request.NotificationType,
                request.UserId
            );
        }

        await _emailLogRepository.AddEmailLogAsync(emailLog);
        await _emailLogRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Substitutes all known placeholders in a raw template string with values from the command.
    /// First replaces the built-in <c>{{recipientName}}</c> token, then iterates over any
    /// caller-supplied <see cref="SendEmailCommand.Placeholders"/>.
    /// </summary>
    private static string ReplacePlaceholders(string template, SendEmailCommand request)
    {
        string result = template.Replace(
            NotificationConstants.TemplatePlaceholders.RECIPIENT_NAME,
            request.RecipientName
        );

        if (request.Placeholders is not null)
        {
            foreach (var (key, value) in request.Placeholders)
            {
                result = result.Replace($"{{{{{key}}}}}", value);
            }
        }

        return result;
    }
}
