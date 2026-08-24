using FluentValidation;
using IdentityService.Application.Features.Auth.DTOs;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Application.Features.Auth.Validators;

/// <summary>
/// Validates the <see cref="ForgotPasswordRequestDto"/> to ensure
/// the required information is provided for initiating the
/// forgot password process.
/// </summary>
public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ValidationConstants.PhoneRequired)
            .Must(phone =>
                string.IsNullOrEmpty(phone) || phone.Length >= ValidationConstants.PhoneMinLength
            )
            .WithMessage(ValidationConstants.PhoneMinLengthMessage);
    }
}
