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
/// Represents a command to verify a user's one-time password (OTP)
/// and generate a password reset token upon successful verification.
/// </summary>
public class VerifyOtpCommand : IRequest<VerifyOtpResponseDto>
{
    /// <summary>
    /// Gets or sets the OTP verification request containing the user's
    /// phone number and OTP.
    /// </summary>
    public VerifyOtpRequestDto Request { get; set; } = null!;
}

/// <summary>
/// Handles the <see cref="VerifyOtpCommand"/> by validating the request,
/// verifying the submitted OTP, generating a password reset token,
/// and invalidating the used OTP.
/// </summary>
public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly IOtpCacheService _otpCacheService;
    private readonly ILogger<VerifyOtpCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyOtpCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">
    /// Repository used to retrieve user information.
    /// </param>
    /// <param name="otpService">
    /// Service responsible for validating one-time passwords (OTPs).
    /// </param>
    /// <param name="otpCacheService">
    /// Service responsible for retrieving, storing, and removing OTPs
    /// and password reset tokens.
    /// </param>
    /// <param name="logger">
    /// Logger used to record OTP verification events.
    /// </param>
    public VerifyOtpCommandHandler(
        IUserRepository userRepository,
        IOtpService otpService,
        IOtpCacheService otpCacheService,
        ILogger<VerifyOtpCommandHandler> logger
    )
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _otpCacheService = otpCacheService;
        _logger = logger;
    }

    /// <summary>
    /// Validates the OTP verification request, verifies the submitted OTP,
    /// generates a password reset token, removes the used OTP,
    /// and returns the generated reset token.
    /// </summary>
    /// <param name="request">
    /// The command containing the OTP verification request.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A response containing the generated password reset token.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when the request validation fails, the OTP has expired,
    /// or the provided OTP is invalid.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no user is registered with the provided phone number.
    /// </exception>
    public async Task<VerifyOtpResponseDto> Handle(
        VerifyOtpCommand request,
        CancellationToken cancellationToken
    )
    {
        var dto = request.Request;

        var user =
            await _userRepository.GetByPhoneAsync(dto.Phone.Trim())
            ?? throw new NotFoundException(ExceptionMessages.PhoneNotRegistered);

        var storedOtp = await _otpCacheService.GetOtpAsync(user.Id);
        if (storedOtp is null)
            throw new BadRequestException(ExceptionMessages.OtpExpired);

        if (!_otpService.ValidateOtp(dto.Otp.Trim(), storedOtp))
            throw new BadRequestException(ExceptionMessages.InvalidOtp);

        var resetToken = Guid.NewGuid().ToString("N");
        await _otpCacheService.SetResetTokenAsync(user.Id, resetToken, TimeSpan.FromMinutes(15));
        await _otpCacheService.RemoveOtpAsync(user.Id);

        _logger.LogInformation("OTP verified for user {UserId}, reset token issued", user.Id);

        return new VerifyOtpResponseDto { ResetToken = resetToken };
    }
}
