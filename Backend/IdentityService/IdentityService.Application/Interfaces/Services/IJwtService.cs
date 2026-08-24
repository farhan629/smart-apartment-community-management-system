using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Services
{
    /// <summary>
    /// Defines methods for generating and validating JWT access and refresh tokens.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT access token for the specified user.
        /// </summary>
        /// <param name="user">
        /// The user for whom the access token is generated.
        /// </param>
        /// <returns>
        /// A signed JWT access token.
        /// </returns>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Generates a refresh token for the specified user.
        /// </summary>
        /// <param name="user">
        /// The user for whom the refresh token is generated.
        /// </param>
        /// <returns>
        /// A refresh token string.
        /// </returns>
        string GenerateRefreshToken(User user);

        /// <summary>
        /// Validates the specified refresh token and extracts the user identifier.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token to validate.
        /// </param>
        /// <returns>
        /// The user identifier if the token is valid; otherwise, <c>null</c>.
        /// </returns>
        Guid? ValidateRefreshToken(string refreshToken);

        /// <summary>
        /// Gets the access token expiration time in minutes.
        /// </summary>
        /// <returns>
        /// The number of minutes an access token remains valid.
        /// </returns>
        int GetAccessTokenExpiryMinutes();

        /// <summary>
        /// Gets the refresh token expiration time in days.
        /// </summary>
        /// <returns>
        /// The number of days a refresh token remains valid.
        /// </returns>
        int GetRefreshTokenExpiryDays();
    }
}