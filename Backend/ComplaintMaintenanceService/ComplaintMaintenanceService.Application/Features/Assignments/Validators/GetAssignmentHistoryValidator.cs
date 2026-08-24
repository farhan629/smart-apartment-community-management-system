using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Validators;

public class GetAssignmentHistoryValidator : AbstractValidator<GetAssignmentHistoryQuery>
{
    public GetAssignmentHistoryValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.ComplaintIdRequired);
    }
}