using FluentValidation;
using IdentityService.Application.Features.Auth.DTOs;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Application.Features.Auth.Validators;

/// <summary>
/// Validates the <see cref="ResetPasswordRequestDto"/> to ensure
/// the reset token is provided and the new password
/// meets the application's password policy.
/// </summary>
public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.ResetToken).NotEmpty().WithMessage(ValidationConstants.ResetTokenRequired);

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
