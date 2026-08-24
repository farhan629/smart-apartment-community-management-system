using System.Security.Claims;
using ComplaintMaintenanceService.Application.Common.Constants;

namespace ComplaintMaintenanceService.API.Helpers;

/// <summary>
/// Shared helper for resolving a resident's own-data scoping info from the current user's claims.
/// </summary>
public static class ResidentScopeHelper
{
    /// <summary>
    /// Returns the resident's own user id if the current user is in the Resident role,
    /// otherwise <c>null</c> (meaning no resident-specific scoping should be applied).
    /// </summary>
    public static Guid? ResolveResidentScopeUserId(ClaimsPrincipal user)
    {
        if (!user.IsInRole(ComplaintConstants.Roles.Resident))
            return null;

        return Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
            ? userId
            : null;
    }
}
