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

namespace IdentityService.Application.Features.Auth.Commands;

/// <summary>
/// Represents a command to reset a user's password using a valid reset token.
/// </summary>
public class ResetPasswordCommand : IRequest<SuccessResponseDto>
{
    /// <summary>
    /// Gets or sets the password reset request containing the reset token
    /// and the new password.
    /// </summary>
    public ResetPasswordRequestDto Request { get; set; } = null!;
}

/// <summary>
/// Handles the <see cref="ResetPasswordCommand"/> by validating the request,
/// verifying the reset token, updating the user's password,
/// and invalidating the used reset token.
/// </summary>
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, SuccessResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IOtpCacheService _otpCacheService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">
    /// Repository used to retrieve and update user credential information.
    /// </param>
    /// <param name="passwordService">
    /// Service used to hash and verify passwords.
    /// </param>
    /// <param name="otpCacheService">
    /// Service responsible for validating and removing password reset tokens.
    /// </param>
    /// <param name="logger">
    /// Logger used to record password reset operations.
    /// </param>
    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IOtpCacheService otpCacheService,
        ILogger<ResetPasswordCommandHandler> logger
    )
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _otpCacheService = otpCacheService;
        _logger = logger;
    }

    /// <summary>
    /// Validates the password reset request, verifies the reset token,
    /// updates the user's password, removes the used reset token,
    /// and returns a success response.
    /// </summary>
    /// <param name="request">
    /// The command containing the password reset request.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A success response indicating that the password has been reset successfully.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when the request validation fails, the reset token is invalid or expired,
    /// the user's credentials cannot be found, or the new password matches the current password.
    /// </exception>
    public async Task<SuccessResponseDto> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var dto = request.Request;


        var userId = await _otpCacheService.GetUserIdByResetTokenAsync(dto.ResetToken.Trim());
        if (userId is null)
            throw new BadRequestException(ExceptionMessages.TokenExpired);

        var credential =
            await _userRepository.GetCredentialByUserIdAsync(userId.Value)
            ?? throw new BadRequestException(ExceptionMessages.CredentialsNotFound);

        if (_passwordService.VerifyPassword(dto.NewPassword, credential.PasswordHash))
            throw new BadRequestException(ExceptionMessages.PasswordReused);

        credential.PasswordHash = _passwordService.HashPassword(dto.NewPassword);
        await _userRepository.UpdateCredentialAsync(credential);

        await _otpCacheService.RemoveResetTokenAsync(dto.ResetToken.Trim());

        _logger.LogInformation("Password reset successfully for user {UserId}", userId.Value);

        return new SuccessResponseDto { Message = "Password changed successfully." };
    }
}
