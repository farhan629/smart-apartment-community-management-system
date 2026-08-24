namespace IdentityService.Application.Features.Auth.DTOs;

/// <summary>
/// Represents the request to initiate the forgot password process
/// using the user's registered phone number.
/// </summary>
public class ForgotPasswordRequestDto
{
    /// <summary>
    /// Gets or sets the registered phone number of the user.
    /// </summary>
    public string Phone { get; set; } = string.Empty;
}
