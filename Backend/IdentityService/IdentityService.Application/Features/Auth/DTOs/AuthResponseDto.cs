using System.Text.Json.Serialization;

namespace IdentityService.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Response DTO containing token details after a successful authentication.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>Gets or sets the JWT access token (returned in body).</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Gets or sets the JWT refresh token (set via HttpOnly cookie).</summary>
        [JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the expiration timestamp of the access token.</summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>Gets or sets the user identifier (set via HttpOnly cookie).</summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
