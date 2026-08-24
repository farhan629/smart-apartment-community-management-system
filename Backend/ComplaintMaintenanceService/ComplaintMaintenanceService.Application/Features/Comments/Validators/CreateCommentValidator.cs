using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Comments.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Comments.Validators;

public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.CommentMessages.ComplaintIdRequired);

        RuleFor(x => x.CommentedBy)
            .NotEmpty()
            .WithMessage(ComplaintConstants.CommentMessages.CommentedByRequired);

        RuleFor(x => x.Request.CommentText)
            .NotEmpty()
            .WithMessage(ComplaintConstants.CommentMessages.CommentTextRequired)
            .MaximumLength(ComplaintConstants.ValidationLimits.CommentTextMaxLength)
            .WithMessage(ComplaintConstants.CommentMessages.CommentTextMaxLength);

        RuleFor(x => x.Request.StaffRating)
            .InclusiveBetween(
                ComplaintConstants.ValidationLimits.StaffRatingMin,
                ComplaintConstants.ValidationLimits.StaffRatingMax
            )
            .WithMessage(ComplaintConstants.CommentMessages.StaffRatingRange)
            .When(x => x.Request.StaffRating.HasValue);
    }
}
