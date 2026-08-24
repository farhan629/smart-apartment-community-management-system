using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Single source of truth for the access-token lifetime.
        /// Required in appsettings.json (Jwt:ExpiryMinutes) — no fallback default.
        /// </summary>
        public int GetAccessTokenExpiryMinutes()
        {
            var raw = _configuration[ConfigKeys.JwtExpiry];

            if (string.IsNullOrEmpty(raw))
            {
                var jwtSettings = _configuration.GetSection(ConfigKeys.JwtSectionName);
                raw = jwtSettings[ConfigKeys.JwtExpiryMinutesSubKey];
            }

            if (!int.TryParse(raw, out var minutes))
                throw new InvalidOperationException(
                    ExceptionMessages.JwtExpiryMinutesNotConfigured
                );

            return minutes;
        }

        public string GenerateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection(ConfigKeys.JwtSectionName);
            var secretKey =
                _configuration[ConfigKeys.JwtKey]
                ?? jwtSettings[ConfigKeys.JwtKeySubKey]
                ?? throw new InvalidOperationException(ExceptionMessages.JwtKeyNotConfigured);
            var issuer =
                _configuration[ConfigKeys.JwtIssuer] ?? jwtSettings[ConfigKeys.JwtIssuerSubKey];
            var audience =
                _configuration[ConfigKeys.JwtAudience] ?? jwtSettings[ConfigKeys.JwtAudienceSubKey];
            var expiryMins = GetAccessTokenExpiryMinutes();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.Code ?? user.RoleId.ToString()),
                new Claim(JwtClaimTypes.RoleId, user.RoleId.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMins),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a short, opaque, cryptographically random refresh token.
        /// Not a JWT — purely a random lookup key validated against the RefreshTokens table.
        /// </summary>
        public string GenerateRefreshToken(User user)
        {
            var randomBytes = new byte[32]; // 256-bit entropy
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert
                .ToBase64String(randomBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        /// <summary>
        /// Returns the refresh-token lifetime in days, used by handlers when
        /// computing ExpiryAt for the RefreshTokens table.
        /// Reads from Jwt:RefreshExpiryDays in config, falls back to 2.
        /// </summary>
        public int GetRefreshTokenExpiryDays()
        {
            var raw = _configuration[
                $"{ConfigKeys.JwtSectionName}:{ConfigKeys.JwtRefreshExpiryDaysSubKey}"
            ];
            if (int.TryParse(raw, out var days))
                return days;
            return ConfigDefaults.RefreshTokenExpiryDaysFallback;
        }

        public Guid? ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            return null;
        }
    }
}
