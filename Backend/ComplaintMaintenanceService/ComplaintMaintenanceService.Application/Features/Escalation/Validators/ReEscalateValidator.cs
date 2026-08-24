using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Escalation.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Escalation.Validators;

public class ReEscalateValidator : AbstractValidator<ReEscalateComplaintCommand>
{
    public ReEscalateValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.EscalationMessages.ComplaintIdRequired);

        RuleFor(x => x.EscalationReason)
            .NotEmpty()
            .WithMessage(ComplaintConstants.EscalationMessages.EscalationReasonRequired)
            .MaximumLength(ComplaintConstants.ValidationLimits.EscalationReasonMaxLength)
            .WithMessage(ComplaintConstants.EscalationMessages.EscalationReasonMaxLength);
    }
}
