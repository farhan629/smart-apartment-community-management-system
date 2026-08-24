namespace ComplaintMaintenanceService.Application.Interfaces.Services;

/// <summary>
/// Abstraction for calling IdentityService via gRPC to retrieve user information.
/// </summary>
public interface IIdentityGrpcClient
{
    /// <summary>
    /// Gets email and name for a single user by their ID.
    /// Returns null if user not found.
    /// </summary>
    Task<UserInfoDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Gets all active users with the given role code.
    /// </summary>
    Task<List<UserInfoDto>> GetUsersByRoleAsync(string roleCode, CancellationToken ct = default);
}

/// <summary>
/// Lightweight user info returned from IdentityService gRPC.
/// </summary>
public record UserInfoDto(Guid UserId, string Email, string Name, string RoleCode);
