using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository contract for managing refresh tokens.
    /// </summary>
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Retrieves the active refresh token associated with the specified user.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the operation.
        /// </param>
        /// <returns>
        /// The active <see cref="RefreshToken"/> if one exists; otherwise, <c>null</c>.
        /// </returns>
        Task<RefreshToken?> GetActiveByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a refresh token by its token key.
        /// </summary>
        /// <param name="tokenKey">
        /// The refresh token value to search for.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the operation.
        /// </param>
        /// <returns>
        /// The matching <see cref="RefreshToken"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<RefreshToken?> GetByTokenKeyAsync(
            string tokenKey,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new refresh token or updates an existing one for the specified user.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user.
        /// </param>
        /// <param name="tokenKey">
        /// The refresh token value.
        /// </param>
        /// <param name="expiryAt">
        /// The date and time when the refresh token expires.
        /// </param>
        /// <param name="performedBy">
        /// The identifier of the user performing the operation.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task UpsertAsync(
            Guid userId,
            string tokenKey,
            DateTime expiryAt,
            Guid? performedBy,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates the refresh token associated with the specified user.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose refresh token should be deactivated.
        /// </param>
        /// <param name="performedBy">
        /// The identifier of the user performing the operation.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task DeactivateAsync(
            Guid userId,
            Guid? performedBy,
            CancellationToken cancellationToken = default);
    }
}