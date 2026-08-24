using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Validators;

public class AcceptAssignmentValidator : AbstractValidator<AcceptAssignmentCommand>
{
    public AcceptAssignmentValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.ComplaintIdRequired);
        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.AssignmentIdRequired);
        RuleFor(x => x.StaffUserId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.StaffUserIdRequired);
    }
}
