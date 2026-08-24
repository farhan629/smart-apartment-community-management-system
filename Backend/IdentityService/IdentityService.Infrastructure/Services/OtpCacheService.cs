using System.Collections.Concurrent;
using IdentityService.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Services;

/// <summary>
/// Provides an in-memory cache for OTPs, reset tokens, and lockout data.
/// </summary>
public class OtpCacheService : IOtpCacheService
{
    private readonly ConcurrentDictionary<string, CacheEntry> _store = new();
    private readonly ILogger<OtpCacheService> _logger;

    /// <summary>
    /// Creates a new instance of the <see cref="OtpCacheService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public OtpCacheService(ILogger<OtpCacheService> logger)
    {
        _logger = logger;
    }

    private static string OtpKey(Guid userId) => $"otp:{userId}";

    private static string ResendKey(Guid userId) => $"resend:{userId}";

    private static string LockKey(Guid userId) => $"lock:{userId}";

    private static string ResetTokenKey(string token) => $"reset_token:{token}";

    /// <inheritdoc/>
    public Task SetOtpAsync(Guid userId, string otp, TimeSpan expiry)
    {
        _store[OtpKey(userId)] = new CacheEntry(otp, DateTime.UtcNow.Add(expiry));
        _logger.LogDebug("OTP set for user {UserId}", userId);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<string?> GetOtpAsync(Guid userId)
    {
        if (_store.TryGetValue(OtpKey(userId), out var entry) && !entry.IsExpired)
            return Task.FromResult<string?>(entry.Value);

        _store.TryRemove(OtpKey(userId), out _);
        return Task.FromResult<string?>(null);
    }

    /// <inheritdoc/>
    public Task RemoveOtpAsync(Guid userId)
    {
        _store.TryRemove(OtpKey(userId), out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SetResetTokenAsync(Guid userId, string resetToken, TimeSpan expiry)
    {
        _store[ResetTokenKey(resetToken)] = new CacheEntry(
            userId.ToString(),
            DateTime.UtcNow.Add(expiry)
        );
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<Guid?> GetUserIdByResetTokenAsync(string resetToken)
    {
        var key = ResetTokenKey(resetToken);
        if (_store.TryGetValue(key, out var entry) && !entry.IsExpired)
        {
            if (Guid.TryParse(entry.Value, out var userId))
                return Task.FromResult<Guid?>(userId);
        }

        _store.TryRemove(key, out _);
        return Task.FromResult<Guid?>(null);
    }

    /// <inheritdoc/>
    public Task RemoveResetTokenAsync(string resetToken)
    {
        _store.TryRemove(ResetTokenKey(resetToken), out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<int> GetResendCountAsync(Guid userId)
    {
        var key = ResendKey(userId);
        if (_store.TryGetValue(key, out var entry) && !entry.IsExpired)
        {
            if (int.TryParse(entry.Value, out var count))
                return Task.FromResult(count);
        }

        return Task.FromResult(0);
    }

    /// <inheritdoc/>
    public Task IncrementResendCountAsync(Guid userId, TimeSpan slidingExpiry)
    {
        var key = ResendKey(userId);
        var now = DateTime.UtcNow;

        if (_store.TryGetValue(key, out var entry) && !entry.IsExpired)
        {
            if (int.TryParse(entry.Value, out var count))
            {
                _store[key] = new CacheEntry((count + 1).ToString(), now.Add(slidingExpiry));
            }
        }
        else
        {
            _store[key] = new CacheEntry("1", now.Add(slidingExpiry));
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ResetResendCountAsync(Guid userId)
    {
        _store.TryRemove(ResendKey(userId), out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> IsLockedAsync(Guid userId)
    {
        var key = LockKey(userId);
        if (_store.TryGetValue(key, out var entry) && !entry.IsExpired)
            return Task.FromResult(true);

        _store.TryRemove(key, out _);
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task SetLockAsync(Guid userId, TimeSpan duration)
    {
        _store[LockKey(userId)] = new CacheEntry("locked", DateTime.UtcNow.Add(duration));
        _logger.LogWarning(
            "User {UserId} locked for OTP until {Until}",
            userId,
            DateTime.UtcNow.Add(duration)
        );
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveLockAsync(Guid userId)
    {
        _store.TryRemove(LockKey(userId), out _);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Represents a cached value with an expiration time.
    /// </summary>
    private class CacheEntry
    {
        /// <summary>
        /// Gets the cached value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the expiration time.
        /// </summary>
        public DateTime ExpiresAt { get; }

        /// <summary>
        /// Gets a value indicating whether the entry has expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;

        /// <summary>
        /// Creates a new cache entry.
        /// /// </summary>
        /// <param name="value">The cached value.</param>
        /// <param name="expiresAt">The expiration time.</param>
        public CacheEntry(string value, DateTime expiresAt)
        {
            Value = value;
            ExpiresAt = expiresAt;
        }
    }
}
