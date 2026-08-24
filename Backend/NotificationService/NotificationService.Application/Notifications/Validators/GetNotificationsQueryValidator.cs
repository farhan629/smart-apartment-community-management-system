using FluentValidation;
using NotificationService.Application.Constants;
using NotificationService.Application.Notifications.Queries.GetNotifications;
using Shared.SharedLibrary.Constants;

namespace NotificationService.Application.Notifications.Validators;

/// <summary>
/// Validates <see cref="GetNotificationsQuery"/> before it reaches the handler.
/// </summary>
public class GetNotificationsQueryValidator : AbstractValidator<GetNotificationsQuery>
{
    /// <summary>
    /// Defines validation rules for <see cref="GetNotificationsQuery"/>.
    /// </summary>
    public GetNotificationsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(NotificationConstants.ValidationMessages.USER_ID_REQUIRED);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(PaginationConstants.MinPageNumber)
            .WithMessage(NotificationConstants.ValidationMessages.PAGE_MUST_BE_POSITIVE);

        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(PaginationConstants.MinPageSize)
            .WithMessage(NotificationConstants.ValidationMessages.LIMIT_MUST_BE_POSITIVE)
            .LessThanOrEqualTo(PaginationConstants.MaxPageSize)
            .WithMessage(NotificationConstants.ValidationMessages.LIMIT_MAX);
    }
}
