using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Validators;

public class GetComplaintsValidator : AbstractValidator<GetComplaintsQuery>
{
    public GetComplaintsValidator()
    {
        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage(ComplaintConstants.Messages.InvalidDateRange)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
    }
}
