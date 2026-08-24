using FluentValidation;
using IdentityService.Application.Features.Auth.DTOs;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Application.Features.Auth.Validators;

/// <summary>
/// Validates the <see cref="LoginRequestDto"/> to ensure
/// all required fields are provided for authentication.
/// </summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(ValidationConstants.EmailRequired);

        RuleFor(x => x.Password).NotEmpty().WithMessage(ValidationConstants.PasswordRequired);
    }
}
