using FluentValidation;
using NotificationService.Application.Constants;
using NotificationService.Application.Notifications.DTOs;

namespace NotificationService.Application.Notifications.Validators;

/// <summary>
/// Validates <see cref="PushNotificationRequestDto"/> before it reaches the handler.
/// </summary>
public class PushNotificationRequestDtoValidator : AbstractValidator<PushNotificationRequestDto>
{
    /// <summary>
    /// Defines validation rules for <see cref="PushNotificationRequestDto"/>:
    /// </summary>
    public PushNotificationRequestDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.USER_ID_REQUIRED);

        RuleFor(x => x.TemplateId)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.TEMPLATE_ID_REQUIRED);

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.TITLE_REQUIRED)
            .MaximumLength(200)
            .WithMessage(NotificationConstants.ValidationMessages.TITLE_MAX_LENGTH);

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.MESSAGE_REQUIRED)
            .MaximumLength(1000)
            .WithMessage(NotificationConstants.ValidationMessages.MESSAGE_MAX_LENGTH);

        RuleFor(x => x.RecipientEmail)
            .EmailAddress()
            .WithMessage(NotificationConstants.ValidationMessages.RECIPIENT_EMAIL_INVALID)
            .When(x => !string.IsNullOrWhiteSpace(x.RecipientEmail));

        RuleFor(x => x.RecipientName)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.RECIPIENT_NAME_REQUIRED)
            .When(x => !string.IsNullOrWhiteSpace(x.RecipientEmail));

        RuleFor(x => x.NotificationType)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.NOTIFICATION_TYPE_REQUIRED);
    }
}
