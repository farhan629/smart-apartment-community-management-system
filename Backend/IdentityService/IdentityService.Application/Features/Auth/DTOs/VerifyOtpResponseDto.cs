namespace IdentityService.Application.Features.Auth.DTOs;

/// <summary>
/// Represents the response returned after successful OTP verification.
/// </summary>
public class VerifyOtpResponseDto
{
    /// <summary>
    /// Gets or sets the password reset token used to authorize
    /// the password reset operation.
    /// </summary>
    public string ResetToken { get; set; } = string.Empty;
}
