using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Validators;

public class AssignComplaintValidator : AbstractValidator<AssignComplaintCommand>
{
    public AssignComplaintValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.ComplaintIdRequired);

        RuleFor(x => x.AssignedBy)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.AssignedByRequired);

        RuleFor(x => x.Request.StaffId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.StaffRequired);

        RuleFor(x => x.Request.DueDate)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.DueDateRequired)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage(ComplaintConstants.AssignmentMessages.DueDateMustBeFuture);
    }
}
