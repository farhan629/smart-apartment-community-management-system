namespace Shared.SharedLibrary.DTO;

/// <summary>
/// Response DTO containing user permissions.
/// </summary>
public class UserPermissionsDto
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the role ID.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the role name.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of permission codes.
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}
