using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Slots.DTO;
using FluentValidation;

namespace AmenityBookingService.Application.Validators;

/// <summary>
/// Validator for <see cref="CreateSlotRequestDto"/>.
/// </summary>
public class CreateSlotRequestValidator : AbstractValidator<CreateSlotRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSlotRequestValidator"/> class.
    /// </summary>
    public CreateSlotRequestValidator()
    {
        RuleFor(x => x.SlotLabel)
            .NotEmpty()
            .WithMessage(ErrorMessages.SlotLableValidation)
            .MaximumLength(100)
            .WithMessage(ErrorMessages.SlotLableLimit);

        RuleFor(x => x.SlotDate)
            .NotEmpty()
            .WithMessage(ErrorMessages.SlotDateValidation)
            .Must(date => date >= DateTime.UtcNow.Date)
            .WithMessage(ErrorMessages.SlotDateValue);

        RuleFor(x => x.StartTime).NotEmpty().WithMessage(ErrorMessages.StartTimeRequired);

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage(ErrorMessages.EndTimeRequired)
            .Must((dto, endTime) => endTime > dto.StartTime)
            .WithMessage(ErrorMessages.EndTimeValidation);

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0)
            .WithMessage(ErrorMessages.MaxCapacityValidation)
            .LessThanOrEqualTo(999)
            .WithMessage(ErrorMessages.MaxCapacityValue);
    }
}
