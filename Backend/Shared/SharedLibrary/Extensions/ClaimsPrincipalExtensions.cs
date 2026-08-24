using System.Security.Claims;

namespace Shared.SharedLibrary.Extensions;

/// <summary>
/// Extension methods for <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the current user ID from the claims principal.
    /// </summary>
    public static Guid GetCurrentUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("sub") ?? user.FindFirst(ClaimTypes.NameIdentifier);

        return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    /// <summary>
    /// Gets the current role ID from the claims principal.
    /// </summary>
    public static Guid GetCurrentRoleId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("roleId") ?? user.FindFirst(ClaimTypes.Role);

        return claim != null && Guid.TryParse(claim.Value, out var roleId) ? roleId : Guid.Empty;
    }
}
