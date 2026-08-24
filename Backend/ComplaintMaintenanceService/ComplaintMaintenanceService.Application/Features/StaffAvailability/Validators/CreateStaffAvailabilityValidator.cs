using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.Commands;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;
using FluentValidation;

namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.Validators;

public class CreateStaffAvailabilityValidator : AbstractValidator<CreateStaffAvailabilityCommand>
{
    public CreateStaffAvailabilityValidator()
    {
        RuleFor(x => x.StaffId)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.StaffIdRequired);

        RuleFor(x => x.Request.Slots)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.SlotsRequired);

        RuleForEach(x => x.Request.Slots).SetValidator(new SlotItemValidator());
    }
}

public class SlotItemValidator : AbstractValidator<SlotItemDto>
{
    public SlotItemValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.DateRequired)
            .Must(d =>
                DateOnly.TryParseExact(d, StaffAvailabilityConstants.DateFormats.SlotDate, out _)
            )
            .WithMessage(StaffAvailabilityConstants.Validation.DateInvalidFormat);

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.StartTimeRequired)
            .Must(t => TimeSpan.TryParse(t, out _))
            .WithMessage(StaffAvailabilityConstants.Validation.StartTimeInvalid);

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage(StaffAvailabilityConstants.Validation.EndTimeRequired)
            .Must(t => TimeSpan.TryParse(t, out _))
            .WithMessage(StaffAvailabilityConstants.Validation.EndTimeInvalid);

        RuleFor(x => x)
            .Must(x =>
            {
                if (!TimeSpan.TryParse(x.StartTime, out var s))
                    return true;
                if (!TimeSpan.TryParse(x.EndTime, out var e))
                    return true;
                return s < e;
            })
            .WithMessage(StaffAvailabilityConstants.Validation.TimeRangeInvalid);
    }
}
