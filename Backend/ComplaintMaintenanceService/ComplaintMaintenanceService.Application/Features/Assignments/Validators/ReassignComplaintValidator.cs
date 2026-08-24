using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Validators;

public class ReassignComplaintValidator : AbstractValidator<ReassignComplaintCommand>
{
    public ReassignComplaintValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.ComplaintIdRequired);

        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.AssignmentIdRequired);

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
