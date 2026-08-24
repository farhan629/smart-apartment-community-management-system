using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.BackgroundJobs.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.BackgroundJobs.Validators;

public class RunEscalationCheckValidator : AbstractValidator<RunEscalationCheckCommand>
{
    public RunEscalationCheckValidator()
    {
        RuleFor(x => x.TriggeredBy)
            .NotEmpty()
            .WithMessage(ComplaintConstants.BackgroundJobMessages.TriggeredByRequired);
    }
}
