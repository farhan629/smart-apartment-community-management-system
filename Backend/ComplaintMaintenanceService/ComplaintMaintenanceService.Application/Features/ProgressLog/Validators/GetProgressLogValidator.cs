using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.ProgressLog.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.ProgressLog.Validators;

public class GetProgressLogValidator : AbstractValidator<GetProgressLogQuery>
{
    public GetProgressLogValidator()
    {
        RuleFor(x => x.ComplaintId)
            .NotEmpty()
            .WithMessage(ComplaintConstants.ProgressLogMessages.ComplaintIdRequired);
    }
}
