namespace AmenityBookingService.Application.Interfaces.Services;

/// <summary>
/// Represents user information retrieved from the Identity service.
/// </summary>
public class IdentityUserInfo
{
    /// <summary>Gets or sets the unique user identifier.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's full name.</summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Defines methods for communicating with the Identity service.
/// </summary>
public interface IIdentityClient
{
    /// <summary>
    /// Retrieves all users assigned to the specified role.
    /// </summary>
    /// <param name="roleCode">The role code (e.g. "Admin").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of users assigned to the specified role.</returns>
    Task<List<IdentityUserInfo>> GetUsersByRoleAsync(
        string roleCode,
        CancellationToken cancellationToken = default
    );
}
