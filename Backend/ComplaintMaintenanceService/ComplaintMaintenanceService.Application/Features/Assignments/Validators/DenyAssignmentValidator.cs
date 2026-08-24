using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Validators;

public class DenyAssignmentValidator : AbstractValidator<DenyAssignmentRequestDto>
{
    public DenyAssignmentValidator()
    {
        RuleFor(x => x.DenialReason)
            .NotEmpty()
            .WithMessage(ComplaintConstants.AssignmentMessages.DenialReasonRequired)
            .MaximumLength(ComplaintConstants.ValidationLimits.DenialReasonMaxLength)
            .WithMessage(ComplaintConstants.AssignmentMessages.DenialReasonMaxLength);
    }
}
