namespace IdentityService.Application.Interfaces.Services;

/// <summary>
/// Defines operations for managing OTPs, password reset tokens,
/// resend attempts, and temporary lockouts using a cache store.
/// </summary>
public interface IOtpCacheService
{
    /// <summary>
    /// Stores an OTP for the specified user with an expiration time.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="otp">The one-time password to store.</param>
    /// <param name="expiry">The duration after which the OTP expires.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetOtpAsync(Guid userId, string otp, TimeSpan expiry);

    /// <summary>
    /// Retrieves the stored OTP for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// A task containing the stored OTP if found; otherwise, <c>null</c>.
    /// </returns>
    Task<string?> GetOtpAsync(Guid userId);

    /// <summary>
    /// Removes the stored OTP for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveOtpAsync(Guid userId);

    /// <summary>
    /// Stores a password reset token for the specified user with an expiration time.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="resetToken">The password reset token.</param>
    /// <param name="expiry">The duration after which the token expires.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetResetTokenAsync(Guid userId, string resetToken, TimeSpan expiry);

    /// <summary>
    /// Retrieves the user identifier associated with the specified password reset token.
    /// </summary>
    /// <param name="resetToken">The password reset token.</param>
    /// <returns>
    /// A task containing the user identifier if the token is valid; otherwise, <c>null</c>.
    /// </returns>
    Task<Guid?> GetUserIdByResetTokenAsync(string resetToken);

    /// <summary>
    /// Removes the specified password reset token from the cache.
    /// </summary>
    /// <param name="resetToken">The password reset token to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveResetTokenAsync(string resetToken);

    /// <summary>
    /// Retrieves the current OTP resend count for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// A task containing the number of OTP resend attempts.
    /// </returns>
    Task<int> GetResendCountAsync(Guid userId);

    /// <summary>
    /// Increments the OTP resend count for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="slidingExpiry">
    /// The sliding expiration duration for the resend count.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task IncrementResendCountAsync(Guid userId, TimeSpan slidingExpiry);

    /// <summary>
    /// Resets the OTP resend count for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ResetResendCountAsync(Guid userId);

    /// <summary>
    /// Determines whether the specified user is currently locked from requesting OTPs.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// A task containing <c>true</c> if the user is locked; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> IsLockedAsync(Guid userId);

    /// <summary>
    /// Applies a temporary lock to the specified user for the given duration.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="duration">The lock duration.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetLockAsync(Guid userId, TimeSpan duration);

    /// <summary>
    /// Removes the temporary lock for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveLockAsync(Guid userId);
}
