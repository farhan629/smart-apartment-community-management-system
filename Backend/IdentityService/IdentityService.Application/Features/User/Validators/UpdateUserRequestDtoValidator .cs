using FluentValidation;
using IdentityService.Application.Features.Users.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

/// <summary>
/// Validates update user request DTO.
/// </summary>
public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes validation rules for username, phone, and photo URL.
    /// </summary>
    /// <param name="userRepository">User repository (optional).</param>
    public UpdateUserRequestDtoValidator(IUserRepository userRepository = null)
    {
        _userRepository = userRepository;

        RuleFor(x => x.UserName)
            .Must(userName =>
                string.IsNullOrEmpty(userName)
                || userName.Length >= ValidationConstants.UsernameMinLength
            )
            .WithMessage(ValidationConstants.UsernameMinLengthMessage)
            .Must(userName =>
                string.IsNullOrEmpty(userName)
                || userName.Length <= ValidationConstants.UsernameMaxLength
            )
            .WithMessage(ValidationConstants.UsernameMaxLengthMessage)
            .Must(userName =>
                string.IsNullOrEmpty(userName)
                || userName.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-')
            )
            .WithMessage(ValidationConstants.UsernameAllowedCharsMessage)
            .When(x => !string.IsNullOrEmpty(x.UserName));

        RuleFor(x => x.Phone)
            .Must(phone =>
                string.IsNullOrEmpty(phone) || phone.Length >= ValidationConstants.PhoneMinLength
            )
            .WithMessage(ValidationConstants.PhoneMinLengthMessage)
            .Must(phone =>
                string.IsNullOrEmpty(phone) || phone.Length <= ValidationConstants.PhoneMaxLength
            )
            .WithMessage(ValidationConstants.PhoneMaxLengthMessage)
            .Must(phone =>
                string.IsNullOrEmpty(phone)
                || phone.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == '(' || c == ')')
            )
            .WithMessage(ValidationConstants.PhoneInvalidCharsMessage)
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.PhotoUrl)
            .Must(url =>
                string.IsNullOrEmpty(url)
                || url.StartsWith("/")
                || Uri.TryCreate(url, UriKind.Absolute, out _)
            )
            .WithMessage(ValidationConstants.PhotoUrlAbsoluteMessage)
            .Must(url =>
                string.IsNullOrEmpty(url)
                || url.StartsWith("/")
                || url.StartsWith(ValidationConstants.PhotoUrlHttpsPrefix)
                || url.StartsWith(ValidationConstants.PhotoUrlHttpPrefix)
            )
            .WithMessage(ValidationConstants.PhotoUrlSchemeMessage)
            .MaximumLength(ValidationConstants.PhotoUrlMaxLength)
            .WithMessage(ValidationConstants.PhotoUrlMaxLengthMessage)
            .When(x => !string.IsNullOrEmpty(x.PhotoUrl));
    }
}
