using FluentValidation;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.Commands;

namespace ResidentVisitorService.Application.Features.Visitors.Validators;

public class UpdateVisitorCommandValidator : AbstractValidator<UpdateVisitorCommand>
{
    public UpdateVisitorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ResidentVisitorConstants.Validation.VisitorIdRequired);

        RuleFor(x => x.Request)
            .Must(r =>
                !string.IsNullOrWhiteSpace(r.Name)
                || !string.IsNullOrWhiteSpace(r.PhoneNumber)
                || r.Email != null
                || r.VisitorTypeId.HasValue
            )
            .WithMessage(ResidentVisitorConstants.Validation.AtLeastOneFieldRequired);

        RuleFor(x => x.Request.Name)
            .MaximumLength(100)
            .WithMessage(ResidentVisitorConstants.Validation.NameTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Name));

        RuleFor(x => x.Request.PhoneNumber)
            .Matches(@"^[0-9]{10}$")
            .WithMessage(ResidentVisitorConstants.Validation.InvalidPhoneNumber)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.PhoneNumber));

        RuleFor(x => x.Request.Email)
            .EmailAddress()
            .WithMessage(ResidentVisitorConstants.Validation.InvalidEmail)
            .MaximumLength(255)
            .WithMessage(ResidentVisitorConstants.Validation.EmailTooLong)
            .When(x => x.Request.Email != null);
    }
}
