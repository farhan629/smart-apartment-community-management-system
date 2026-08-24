using FluentValidation;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.DTOs;

namespace ResidentVisitorService.Application.Features.Visitors.Validators;

/// <summary>
/// Validates a <see cref="CreateVisitorRequestDto"/>. Used both when creating a visitor
/// directly and when creating a visit with an inline (on-the-fly) visitor.
/// </summary>
public class CreateVisitorRequestDtoValidator : AbstractValidator<CreateVisitorRequestDto>
{
    public CreateVisitorRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.NameRequired)
            .MaximumLength(100)
            .WithMessage(ResidentVisitorConstants.Validation.NameTooLong);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.PhoneNumberRequired)
            .Matches(@"^[0-9]{10}$")
            .WithMessage(ResidentVisitorConstants.Validation.InvalidPhoneNumber);

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(ResidentVisitorConstants.Validation.InvalidEmail)
            .MaximumLength(255)
            .WithMessage(ResidentVisitorConstants.Validation.EmailTooLong)
            .When(x => x.Email != null);

        RuleFor(x => x.VisitorTypeId)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.VisitorTypeIdRequired);
    }
}
