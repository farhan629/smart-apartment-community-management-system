namespace IdentityService.Application.Features.Auth.DTOs;

/// <summary>
/// Represents the request to reset a user's password using a valid reset token.
/// </summary>
public class ResetPasswordRequestDto
{
    /// <summary>
    /// Gets or sets the password reset token issued after successful OTP verification.
    /// </summary>
    public string ResetToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password that will replace the existing password.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confirmation of the new password.
    /// This value should match <see cref="NewPassword"/>.
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
