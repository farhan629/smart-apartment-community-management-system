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
/// Represents a command to initiate the forgot password process
/// by generating and sending an OTP to the user's registered phone number.
/// </summary>
public class ForgotPasswordCommand : IRequest<SuccessResponseDto>
{
    /// <summary>
    /// Gets or sets the forgot password request containing the user's phone number.
    /// </summary>
    public ForgotPasswordRequestDto Request { get; set; } = null!;
}

/// <summary>
/// Handles the <see cref="ForgotPasswordCommand"/> by validating the request,
/// generating a one-time password (OTP), storing it temporarily,
/// and sending it to the user's registered phone number.
/// </summary>
public class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, SuccessResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly IOtpCacheService _otpCacheService;
    private readonly ISmsService _smsService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">
    /// Repository used to retrieve user information.
    /// </param>
    /// <param name="otpService">
    /// Service responsible for generating one-time passwords (OTPs).
    /// </param>
    /// <param name="otpCacheService">
    /// Service responsible for storing OTPs, tracking resend attempts,
    /// and enforcing temporary lockouts.
    /// </param>
    /// <param name="smsService">
    /// Service used to send OTPs via SMS.
    /// </param>
    /// <param name="logger">
    /// Logger used to record forgot password operations.
    /// </param>
    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IOtpService otpService,
        IOtpCacheService otpCacheService,
        ISmsService smsService,
        ILogger<ForgotPasswordCommandHandler> logger
    )
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _otpCacheService = otpCacheService;
        _smsService = smsService;
        _logger = logger;
    }

    /// <summary>
    /// Validates the forgot password request, checks OTP resend limits,
    /// generates a new OTP, stores it temporarily,
    /// and sends it to the user's registered phone number.
    /// </summary>
    /// <param name="request">
    /// The command containing the forgot password request.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A success response indicating that the OTP has been sent successfully.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when the request validation fails, the user has exceeded
    /// the allowed OTP resend attempts, or the user is temporarily locked.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no user is registered with the provided phone number.
    /// </exception>
    public async Task<SuccessResponseDto> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var dto = request.Request;

        var user =
            await _userRepository.GetByPhoneAsync(dto.Phone.Trim())
            ?? throw new NotFoundException(ExceptionMessages.PhoneNotRegistered);

        if (await _otpCacheService.IsLockedAsync(user.Id))
            throw new BadRequestException(ExceptionMessages.TooManyOtpAttempts);

        var resendCount = await _otpCacheService.GetResendCountAsync(user.Id);
        if (resendCount >= 3)
        {
            await _otpCacheService.SetLockAsync(user.Id, TimeSpan.FromHours(24));
            throw new BadRequestException(ExceptionMessages.TooManyOtpAttempts);
        }

        var otp = _otpService.GenerateOtp();
        await _otpCacheService.SetOtpAsync(user.Id, otp, TimeSpan.FromMinutes(10));
        await _otpCacheService.IncrementResendCountAsync(user.Id, TimeSpan.FromHours(24));

        await _smsService.SendSmsAsync(
            user.PhoneNo,
            $"Your OTP for password reset is: {otp}. Valid for 10 minutes."
        );

        _logger.LogInformation("OTP sent to user {UserId} for password reset", user.Id);

        return new SuccessResponseDto { Message = "OTP sent to your registered phone number." };
    }
}
