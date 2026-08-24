using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface for the repository managing reference sets.
    /// </summary>
    public interface IRefSetRepository
    {
        /// <summary>
        /// Retrieves a reference set by its unique name.
        /// </summary>
        /// <param name="setName">The name of the reference set.</param>
        /// <returns>The reference set entity, or null if not found.</returns>
        Task<RefSet?> GetBySetNameAsync(string setName);

        /// <summary>
        /// Adds a new reference set to the repository.
        /// </summary>
        /// <param name="refSet">The reference set entity to add.</param>
        /// <returns>The added reference set entity.</returns>
        Task<RefSet> AddAsync(RefSet refSet);
    }
}
