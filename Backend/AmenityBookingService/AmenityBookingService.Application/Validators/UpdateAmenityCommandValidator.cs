using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Amenities.DTO;
using FluentValidation;

namespace AmenityBookingService.Application.Validators;

/// <summary>
/// Validates requests for updating an existing amenity.
/// </summary>
public class UpdateAmenityRequestValidator : AbstractValidator<UpdateAmenityRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAmenityRequestValidator"/> class
    /// and defines validation rules for <see cref="UpdateAmenityRequestDto"/>.
    /// </summary>
    public UpdateAmenityRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(255)
            .WithMessage(ErrorMessages.NameLimitError)
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Location)
            .MaximumLength(255)
            .WithMessage(ErrorMessages.LocationStringLimit)
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.Rules)
            .MaximumLength(2000)
            .WithMessage(ErrorMessages.RuleStringLimit)
            .When(x => !string.IsNullOrEmpty(x.Rules));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500)
            .WithMessage(ErrorMessages.ImageStringLimit)
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x).Must(HaveAtLeastOneField).WithMessage(ErrorMessages.AtLeastOneUpdateError);
    }

    /// <summary>
    /// Determines whether the update request contains at least one field to modify.
    /// </summary>
    /// <param name="dto">The amenity update request.</param>
    /// <returns>
    /// <see langword="true"/> if at least one updatable field is provided; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    private static bool HaveAtLeastOneField(UpdateAmenityRequestDto dto)
    {
        return !string.IsNullOrEmpty(dto.Name)
            || dto.SlotTypeId.HasValue
            || dto.StatusId.HasValue
            || !string.IsNullOrEmpty(dto.Location)
            || !string.IsNullOrEmpty(dto.Rules)
            || !string.IsNullOrEmpty(dto.ImageUrl);
    }
}
