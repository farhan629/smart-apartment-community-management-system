using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Amenities.DTO;
using FluentValidation;

namespace AmenityBookingService.Application.Validators;

/// <summary>
/// Validates requests for creating a new amenity.
/// </summary>
public class CreateAmenityRequestValidator : AbstractValidator<CreateAmenityRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAmenityRequestValidator"/> class
    /// and defines validation rules for <see cref="CreateAmenityRequestDto"/>.
    /// </summary>
    public CreateAmenityRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessages.NameRequired)
            .MaximumLength(255)
            .WithMessage(ErrorMessages.NameLimitError);

        RuleFor(x => x.SlotTypeId).NotEmpty().WithMessage(ErrorMessages.SlotTypeRequiredError);

        RuleFor(x => x.StatusId).NotEmpty().WithMessage(ErrorMessages.StatusIdRequired);

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage(ErrorMessages.LocationRequired)
            .MaximumLength(255)
            .WithMessage(ErrorMessages.LocationStringLimit);

        RuleFor(x => x.Rules).MaximumLength(2000).WithMessage(ErrorMessages.RuleStringLimit);

        RuleFor(x => x.ImageUrl).MaximumLength(500).WithMessage(ErrorMessages.ImageStringLimit);
    }
}
