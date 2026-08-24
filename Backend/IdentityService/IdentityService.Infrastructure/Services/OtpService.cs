using System.Security.Cryptography;
using IdentityService.Application.Interfaces.Services;

namespace IdentityService.Infrastructure.Services;

/// <summary>
/// Provides OTP generation and validation services.
/// </summary>
public class OtpService : IOtpService
{
    /// <inheritdoc/>
    public string GenerateOtp()
    {
        var bytes = RandomNumberGenerator.GetBytes(4);
        var val = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
        return val.ToString("D6");
    }

    /// <inheritdoc/>
    public bool ValidateOtp(string inputOtp, string storedOtp)
    {
        return string.Equals(inputOtp?.Trim(), storedOtp?.Trim(), StringComparison.Ordinal);
    }
}
