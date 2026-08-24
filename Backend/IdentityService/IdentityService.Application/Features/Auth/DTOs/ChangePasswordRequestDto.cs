namespace IdentityService.Application.Features.Auth.DTOs;

/// <summary>
/// Represents the request to change the password of the currently authenticated user.
/// </summary>
public class ChangePasswordRequestDto
{
    /// <summary>
    /// Gets or sets the user's current password for verification.
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password that will replace the current password.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confirmation of the new password.
    /// This value should match <see cref="NewPassword"/>.
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
