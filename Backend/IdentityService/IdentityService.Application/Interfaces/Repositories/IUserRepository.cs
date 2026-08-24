using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface for the repository managing User entities and credentials.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user entity, or null if not found.</returns>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a user by their unique email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user entity, or null if not found.</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Retrieves a user by their phone number.
        /// </summary>
        /// <param name="phone">The phone number of the user.</param>
        /// <returns>The user entity, or null if not found.</returns>
        Task<User?> GetByPhoneAsync(string phone);

        /// <summary>
        /// Retrieves a user along with their password security credentials by email.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user entity with credentials, or null if not found.</returns>
        Task<User?> GetUserWithCredentialAsync(string email);

        /// <summary>
        /// Gets a paginated list of all users.
        /// </summary>
        /// <param name="page">The page number (1-indexed).</param>
        /// <param name="limit">The number of items per page.</param>
        /// <returns>A tuple containing total count and the collection of user entities.</returns>
        Task<(int Total, IEnumerable<User> Items)> GetAllUsersAsync(
            int page,
            int limit,
            string? name = null,
            Guid? roleId = null
        );

        /// <summary>
        /// Checks if a user exists with the specified email address.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>True if the email exists, otherwise false.</returns>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Adds a new user to the repository.
        /// </summary>
        /// <param name="user">The user entity to add.</param>
        /// <returns>The added user entity.</returns>
        Task<User> AddAsync(User user);

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="user">The user entity to update.</param>
        Task UpdateAsync(User user);

        /// <summary>
        /// Retrieves password security details for a specific user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The password security entity, or null if not found.</returns>
        Task<UserPasswordSecurity?> GetCredentialByUserIdAsync(Guid userId);

        /// <summary>
        /// Updates a user's password security credentials.
        /// </summary>
        /// <param name="credential">The user password security entity to update.</param>
        Task UpdateCredentialAsync(UserPasswordSecurity credential);

        /// <summary>
        /// Adds a user along with their password security details as a transaction.
        /// </summary>
        /// <param name="user">The user entity to add.</param>
        /// <param name="credential">The user password security credentials to add.</param>
        /// <returns>The added user entity.</returns>
        Task<User> AddUserWithCredentialAsync(User user, UserPasswordSecurity credential);

        /// <summary>
        /// Soft-deletes a user from the repository by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>True if successful, otherwise false.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves a user along with their role details.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The user entity with role details, or null if not found.</returns>
        Task<User?> GetUserWithRoleAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Retrieves the list of permission policy strings associated with a specific role ID.
        /// </summary>
        /// <param name="roleId">The unique identifier of the role.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of permission strings.</returns>
        Task<List<string>> GetRolePermissionsAsync(
            Guid roleId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Retrieves the list of policies directly associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of user policy entities.</returns>
        Task<List<UserPolicy>> GetUserPoliciesAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Retrieves all active users that have the specified role ID.
        /// </summary>
        /// <param name="roleId">The role identifier to filter by.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of user entities with the given role.</returns>
        Task<List<User>> GetUsersByRoleIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Upserts a batch of user policies (overrides) for a specific user.
        /// Creates new entries or updates existing ones based on (UserId, PermissionCode) unique constraint.
        /// </summary>
        /// <param name="policies">The list of user policy entries to upsert.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task UpsertUserPoliciesAsync(
            IEnumerable<UserPolicy> policies,
            CancellationToken cancellationToken = default
        );
    }
}
