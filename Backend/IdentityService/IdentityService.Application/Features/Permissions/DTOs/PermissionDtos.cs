namespace IdentityService.Application.Features.Permissions.DTOs;

/// <summary>
/// Response DTO for user permissions
/// </summary>
public class UserPermissionsResponseDto
{
    public Guid UserId { get; set; }
    public List<string> Permissions { get; set; } = new();
}

/// <summary>
/// Request DTO for assigning permissions to a user
/// </summary>
public class AssignUserPermissionsRequestDto
{
    public Guid UserId { get; set; }
    public List<UserPermissionEntryDto> Permissions { get; set; } = new();
}

/// <summary>
/// Permission entry DTO
/// </summary>
public class UserPermissionEntryDto
{
    public string PermissionCode { get; set; } = string.Empty;
    public bool IsAllowed { get; set; }
}
