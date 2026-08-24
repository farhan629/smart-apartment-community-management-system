namespace Shared.SharedLibrary.Services;

/// <summary>
/// Service for managing user permissions.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Gets all permissions for a user.
    /// </summary>
    Task<List<string>> GetUserPermissionsAsync(Guid userId);

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permissionCode);
}
