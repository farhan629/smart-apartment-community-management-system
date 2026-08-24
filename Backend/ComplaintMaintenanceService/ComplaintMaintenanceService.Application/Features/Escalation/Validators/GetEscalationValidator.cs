using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Escalation.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Escalation.Validators;

public class GetEscalationValidator : AbstractValidator<GetEscalationQuery>
{
    public GetEscalationValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.EscalationMessages.ComplaintIdRequired);
    }
}
