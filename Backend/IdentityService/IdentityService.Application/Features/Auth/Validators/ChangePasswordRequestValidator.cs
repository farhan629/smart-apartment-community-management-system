using FluentValidation;
using IdentityService.Application.Features.Auth.DTOs;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Application.Features.Auth.Validators;

/// <summary>
/// Validates the <see cref="ChangePasswordRequestDto"/> to ensure
/// all required fields are provided and the new password
/// meets the application's password policy.
/// </summary>
public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage(ValidationConstants.CurrentPasswordRequired);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(ValidationConstants.NewPasswordRequired)
            .MinimumLength(ValidationConstants.PasswordMinLength)
            .WithMessage(ValidationConstants.PasswordMinLengthMessage)
            .Must(password => password.Any(char.IsUpper))
            .WithMessage(ValidationConstants.PasswordUppercaseMessage)
            .Must(password => password.Any(char.IsDigit))
            .WithMessage(ValidationConstants.PasswordDigitMessage)
            .Must(password => password.Any(ch => !char.IsLetterOrDigit(ch)))
            .WithMessage(ValidationConstants.PasswordSpecialCharMessage);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage(ValidationConstants.ConfirmPasswordRequired)
            .Equal(x => x.NewPassword)
            .WithMessage(ValidationConstants.PasswordsDoNotMatch);
    }
}
