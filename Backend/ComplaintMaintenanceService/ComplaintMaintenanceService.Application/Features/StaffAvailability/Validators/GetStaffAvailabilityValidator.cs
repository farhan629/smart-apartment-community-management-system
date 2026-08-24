using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.Validators;

public class GetStaffAvailabilityValidator : AbstractValidator<GetStaffAvailabilityQuery>
{
    public GetStaffAvailabilityValidator()
    {
        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage(StaffAvailabilityConstants.Validation.DateRangeInvalid)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage(StaffAvailabilityConstants.Validation.FilterTimeRangeInvalid)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue);
    }
}
