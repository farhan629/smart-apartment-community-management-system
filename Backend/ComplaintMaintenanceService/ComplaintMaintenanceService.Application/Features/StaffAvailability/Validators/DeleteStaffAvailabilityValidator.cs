using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.Commands;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.Validators;

public class DeleteStaffAvailabilityValidator : AbstractValidator<DeleteStaffAvailabilityCommand>
{
    public DeleteStaffAvailabilityValidator()
    {
        RuleFor(x => x.SlotId)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.SlotIdRequired);
        RuleFor(x => x.StaffId)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.StaffIdRequired);
    }
}
