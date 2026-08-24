using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.Queries;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.Validators;

public class GetStaffAvailabilityByIdValidator : AbstractValidator<GetStaffAvailabilityByIdQuery>
{
    public GetStaffAvailabilityByIdValidator()
    {
        RuleFor(x => x.SlotId)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.SlotIdRequired);
        RuleFor(x => x.StaffId)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.StaffIdRequired);
    }
}
