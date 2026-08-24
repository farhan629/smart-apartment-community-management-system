using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Comments.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Comments.Validators;

public class GetCommentsValidator : AbstractValidator<GetCommentsQuery>
{
    public GetCommentsValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.CommentMessages.ComplaintIdRequired);
    }
}
