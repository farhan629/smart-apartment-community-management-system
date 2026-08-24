using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Bookings.DTO;
using FluentValidation;

namespace AmenityBookingService.Application.Validators;

/// <summary>
/// Validates requests for creating a new amenity booking.
/// </summary>
public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBookingRequestValidator"/> class
    /// and defines validation rules for <see cref="CreateBookingRequestDto"/>.
    /// </summary>
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.SlotId).NotEmpty().WithMessage(ErrorMessages.SlotIdRequired);

        RuleFor(x => x.PeopleCount)
            .GreaterThan(0)
            .WithMessage(ErrorMessages.PeopleCountMinError)
            .LessThanOrEqualTo(999)
            .WithMessage(ErrorMessages.PeopleCountMaxError);
    }
}
