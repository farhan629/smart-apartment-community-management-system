using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Slots.DTO;
using FluentValidation;

namespace AmenityBookingService.Application.Validators;

/// <summary>
/// Validates requests for updating an existing amenity slot.
/// </summary>
public class UpdateSlotRequestValidator : AbstractValidator<UpdateSlotRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSlotRequestValidator"/> class
    /// and defines validation rules for <see cref="UpdateSlotRequestDto"/>.
    /// </summary>
    public UpdateSlotRequestValidator()
    {
        RuleFor(x => x.SlotLabel)
            .MaximumLength(100)
            .WithMessage(ErrorMessages.SlotLabelLimit)
            .When(x => !string.IsNullOrEmpty(x.SlotLabel));

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0)
            .WithMessage(ErrorMessages.MaxCapacityLimit)
            .LessThanOrEqualTo(999)
            .WithMessage(ErrorMessages.MaxCapacityMaxLimit)
            .When(x => x.MaxCapacity.HasValue);

        RuleFor(x => x).Must(HaveAtLeastOneField).WithMessage(ErrorMessages.AtLeastOneUpdateError);
    }

    /// <summary>
    /// Determines whether the update request contains at least one field to modify.
    /// </summary>
    /// <param name="dto">The slot update request.</param>
    /// <returns>
    /// <see langword="true"/> if at least one updatable field is provided; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    private static bool HaveAtLeastOneField(UpdateSlotRequestDto dto)
    {
        return !string.IsNullOrEmpty(dto.SlotLabel)
            || dto.StartTime.HasValue
            || dto.EndTime.HasValue
            || dto.MaxCapacity.HasValue;
    }
}
