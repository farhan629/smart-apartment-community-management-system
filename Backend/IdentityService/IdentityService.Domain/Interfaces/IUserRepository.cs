using IdentityService.Domain.Entities;

namespace IdentityService.Domain.Interfaces.Repositories;

/// <summary>
/// Interface for the Domain layer User repository.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user along with their role details.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user entity, or null if not found.</returns>
    Task<User?> GetUserWithRoleAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all permission policy strings associated with a specific role ID.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of permission strings.</returns>
    Task<List<string>> GetRolePermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves override policies associated directly with a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of user policy override entities.</returns>
    Task<List<UserPolicy>> GetUserPoliciesAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
