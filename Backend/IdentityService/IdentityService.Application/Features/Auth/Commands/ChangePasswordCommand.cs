using FluentValidation;
using IdentityService.Application.Features.Auth.DTOs;
using IdentityService.Application.Features.Auth.Validators;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace IdentityService.Application.Features.Auth.Commands;

/// <summary>
/// Represents a command to change the password of the currently authenticated user.
/// </summary>
public class ChangePasswordCommand : IRequest<SuccessResponseDto>
{
    /// <summary>
    /// Gets or sets the password change request containing the current and new passwords.
    /// </summary>
    public ChangePasswordRequestDto Request { get; set; } = null!;
}

/// <summary>
/// Handles the <see cref="ChangePasswordCommand"/> by validating the request,
/// verifying the current password, updating the password hash,
/// and persisting the changes.
/// </summary>
public class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, SuccessResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangePasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">
    /// Repository used to retrieve and update user credential information.
    /// </param>
    /// <param name="passwordService">
    /// Service used to hash and verify passwords.
    /// </param>
    /// <param name="currentUserService">
    /// Service that provides information about the currently authenticated user.
    /// </param>
    /// <param name="logger">
    /// Logger used to record password change events.
    /// </param>
    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ICurrentUserService currentUserService,
        ILogger<ChangePasswordCommandHandler> logger
    )
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Validates the password change request, verifies the user's current password,
    /// updates the password hash, and saves the changes.
    /// </summary>
    /// <param name="request">
    /// The command containing the password change request.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A success response indicating that the password has been changed successfully.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when the request validation fails, user credentials are not found,
    /// the current password is incorrect, or the new password matches the existing password.
    /// </exception>
    public async Task<SuccessResponseDto> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var dto = request.Request;


        var credential =
            await _userRepository.GetCredentialByUserIdAsync(_currentUserService.UserId)
            ?? throw new BadRequestException(ExceptionMessages.CredentialsNotFound);

        if (!_passwordService.VerifyPassword(dto.CurrentPassword, credential.PasswordHash))
            throw new BadRequestException(ExceptionMessages.CurrentPasswordIncorrect);

        if (_passwordService.VerifyPassword(dto.NewPassword, credential.PasswordHash))
            throw new BadRequestException(ExceptionMessages.PasswordReused);

        credential.PasswordHash = _passwordService.HashPassword(dto.NewPassword);
        await _userRepository.UpdateCredentialAsync(credential);

        _logger.LogInformation(
            "Password changed successfully for user {UserId}",
            _currentUserService.UserId
        );

        return new SuccessResponseDto { Message = "Password changed successfully." };
    }
}
