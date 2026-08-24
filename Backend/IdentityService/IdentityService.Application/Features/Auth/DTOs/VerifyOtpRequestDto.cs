namespace IdentityService.Application.Features.Auth.DTOs;

/// <summary>
/// Represents the request to verify a one-time password (OTP)
/// for the password reset process.
/// </summary>
public class VerifyOtpRequestDto
{
    /// <summary>
    /// Gets or sets the registered phone number of the user.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the one-time password (OTP) sent to the user's registered phone number.
    /// </summary>
    public string Otp { get; set; } = string.Empty;
}
