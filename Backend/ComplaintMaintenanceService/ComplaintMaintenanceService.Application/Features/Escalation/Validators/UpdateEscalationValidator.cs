using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Escalation.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Escalation.Validators;

public class UpdateEscalationValidator : AbstractValidator<UpdateEscalationCommand>
{
    public UpdateEscalationValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.EscalationMessages.ComplaintIdRequired);

        RuleFor(x => x.UpdatedBy)
            .NotEmpty()
            .WithMessage(ComplaintConstants.EscalationMessages.UpdatedByRequired);

        RuleFor(x => x.Request.ResolutionDate)
            .NotEmpty()
            .WithMessage(ComplaintConstants.EscalationMessages.ResolutionDateRequiredWhenResolved)
            .When(x => x.Request.ResolvedAfterEscalation);

        RuleFor(x => x.Request.ResolutionDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage(ComplaintConstants.EscalationMessages.ResolutionDateCannotBeFuture)
            .When(x => x.Request.ResolutionDate.HasValue);
    }
}