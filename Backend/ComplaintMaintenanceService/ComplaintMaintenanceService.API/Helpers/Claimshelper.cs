using System.Security.Claims;

namespace ComplaintMaintenanceService.API.Helpers;

/// <summary>
/// Shared helper for safely parsing <see cref="Guid"/> values out of user claims.
/// </summary>
public static class ClaimsHelper
{
    /// <summary>
    /// Returns the parsed <see cref="Guid"/> value of the given claim type, or
    /// <see cref="Guid.Empty"/> if the claim is missing or not a valid Guid.
    /// </summary>
    public static Guid GetGuidClaimOrEmpty(ClaimsPrincipal user, string claimType)
    {
        var claimValue = user.FindFirstValue(claimType);
        return Guid.TryParse(claimValue, out var parsed) ? parsed : Guid.Empty;
    }
}
