using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Comments.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Comments.Validators;

public class GetStaffCommentsValidator : AbstractValidator<GetStaffCommentsQuery>
{
    public GetStaffCommentsValidator()
    {
        RuleFor(x => x.StaffId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.CommentMessages.StaffIdRequired);
    }
}
