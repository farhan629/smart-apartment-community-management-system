using FluentValidation;
using NotificationService.Application.Constants;
using NotificationService.Application.Notifications.Commands.SendEmail;

namespace NotificationService.Application.Notifications.Validators;

/// <summary>
/// Validates <see cref="SendEmailCommand"/> before it reaches the handler.
/// </summary>
public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    /// <summary>
    /// Defines validation rules for <see cref="SendEmailCommand"/>:
    /// UserId, NotificationType, and RecipientName are required;
    /// RecipientEmail is required and must be a valid email address.
    /// </summary>
    public SendEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.USER_ID_REQUIRED);

        RuleFor(x => x.NotificationType)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.NOTIFICATION_TYPE_REQUIRED);

        RuleFor(x => x.RecipientEmail)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.RECIPIENT_EMAIL_REQUIRED)
            .EmailAddress()
            .WithMessage(NotificationConstants.ValidationMessages.RECIPIENT_EMAIL_INVALID);

        RuleFor(x => x.RecipientName)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.RECIPIENT_NAME_REQUIRED);
    }
}
