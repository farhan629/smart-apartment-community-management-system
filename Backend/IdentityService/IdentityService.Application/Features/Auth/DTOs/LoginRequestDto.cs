namespace IdentityService.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Request DTO containing the user's login credentials.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>Gets or sets the user's unique email address.</summary>
        public string? Email { get; set; }

        /// <summary>Gets or sets the user's plain-text password.</summary>
        public string? Password { get; set; }
    }
}
