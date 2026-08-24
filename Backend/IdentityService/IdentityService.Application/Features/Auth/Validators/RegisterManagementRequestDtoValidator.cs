using System.Text.RegularExpressions;
using FluentValidation;
using IdentityService.Application.Features.Auth.DTOs;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Application.Features.Auth.Validators;

/// <summary>
/// Validates the <see cref="RegisterManagementRequestDto"/> to ensure
/// all required fields are provided and correctly formatted
/// for management user (Admin / Staff) registration.
/// </summary>
public class RegisterManagementRequestDtoValidator : AbstractValidator<RegisterManagementRequestDto>
{
    private static readonly Regex EmailRegex = new Regex(
        ValidationConstants.EmailRegexPattern,
        RegexOptions.Compiled
    );

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterManagementRequestDtoValidator"/> class
    /// and defines validation rules for management user registration requests.
    /// </summary>
    public RegisterManagementRequestDtoValidator()
    {
        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationConstants.EmailRequired)
            .MaximumLength(ValidationConstants.EmailMaxLength)
            .WithMessage(ValidationConstants.EmailMaxLengthMessage)
            .Must(BeValidEmail)
            .WithMessage(email => GetEmailErrorMessage(email.Email));

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ValidationConstants.PasswordRequired)
            .MinimumLength(ValidationConstants.PasswordMinLength)
            .WithMessage(ValidationConstants.PasswordMinLengthMessage)
            .Must(password => password.Any(char.IsUpper))
            .WithMessage(ValidationConstants.PasswordUppercaseMessage)
            .Must(password => password.Any(char.IsDigit))
            .WithMessage(ValidationConstants.PasswordDigitMessage)
            .Must(password => password.Any(ch => !char.IsLetterOrDigit(ch)))
            .WithMessage(ValidationConstants.PasswordSpecialCharMessage);

        // Phone validation
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ValidationConstants.PhoneRequired)
            .Must(phone =>
                string.IsNullOrEmpty(phone) || phone.Length >= ValidationConstants.PhoneMinLength
            )
            .WithMessage(ValidationConstants.PhoneMinLengthMessage);

        // Role validation
        RuleFor(x => x.Role_id).NotNull().WithMessage(ExceptionMessages.InvalidRole);

        // Category validation
        RuleFor(x => x.category_id)
            .NotNull()
            .WithMessage(ExceptionMessages.RoleAlreadyExistsInCategory);
    }

    private static bool BeValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }

    private static string GetEmailErrorMessage(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ValidationConstants.EmailRequired;

        return ValidationConstants.EmailInvalidFormatMessage;
    }
}
