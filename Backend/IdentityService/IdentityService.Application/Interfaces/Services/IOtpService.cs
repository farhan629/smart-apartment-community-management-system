namespace IdentityService.Application.Interfaces.Services;

/// <summary>
/// Defines operations for generating and validating one-time passwords (OTPs).
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generates a new one-time password (OTP).
    /// </summary>
    /// <returns>
    /// A randomly generated OTP.
    /// </returns>
    string GenerateOtp();

    /// <summary>
    /// Validates the user-provided OTP against the stored OTP.
    /// </summary>
    /// <param name="inputOtp">
    /// The OTP entered by the user.
    /// </param>
    /// <param name="storedOtp">
    /// The OTP stored by the system for comparison.
    /// </param>
    /// <returns>
    /// <c>true</c> if the OTPs match; otherwise, <c>false</c>.
    /// </returns>
    bool ValidateOtp(string inputOtp, string storedOtp);
}
