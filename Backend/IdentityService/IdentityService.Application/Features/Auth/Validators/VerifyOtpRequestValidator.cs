using FluentValidation;
using IdentityService.Application.Features.Auth.DTOs;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Application.Features.Auth.Validators;

/// <summary>
/// Validates the <see cref="VerifyOtpRequestDto"/> to ensure
/// the required information is provided and the OTP is in the expected format.
/// </summary>
public class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequestDto>
{
    public VerifyOtpRequestValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ValidationConstants.PhoneRequired)
            .Must(phone =>
                string.IsNullOrEmpty(phone) || phone.Length >= ValidationConstants.PhoneMinLength
            )
            .WithMessage(ValidationConstants.PhoneMinLengthMessage);

        RuleFor(x => x.Otp)
            .NotEmpty()
            .WithMessage(ValidationConstants.OtpRequired)
            .Length(ValidationConstants.OtpLength)
            .WithMessage(ValidationConstants.OtpLengthMessage)
            .Matches(ValidationConstants.OtpRegexPattern)
            .WithMessage(ValidationConstants.OtpDigitsOnlyMessage);
    }
}
