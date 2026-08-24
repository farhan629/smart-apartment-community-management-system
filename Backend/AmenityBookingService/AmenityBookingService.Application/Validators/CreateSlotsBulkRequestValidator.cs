using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Slots.DTO;
using FluentValidation;

namespace AmenityBookingService.Application.Validators;

/// <summary>
/// Validates requests for creating multiple amenity slots in a single operation.
/// </summary>
public class CreateSlotsBulkRequestValidator : AbstractValidator<CreateSlotsBulkRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSlotsBulkRequestValidator"/> class
    /// and defines validation rules for <see cref="CreateSlotsBulkRequestDto"/>.
    /// </summary>
    public CreateSlotsBulkRequestValidator()
    {
        RuleFor(x => x.Slots)
            .NotEmpty()
            .WithMessage(ErrorMessages.CreateSlotBlukMinError)
            .Must(slots => slots.Count <= 100)
            .WithMessage(ErrorMessages.CreateSlotBulkMaxError);

        RuleForEach(x => x.Slots).SetValidator(new CreateSlotRequestValidator());
    }
}
