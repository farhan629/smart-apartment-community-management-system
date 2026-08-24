using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface for the repository managing reference sets.
    /// </summary>
    public interface IRefSetRepository
    {
        /// <summary>
        /// Retrieves a reference set by its unique code.
        /// </summary>
        /// <param name="code">The code of the reference set (e.g. "COMPLAINT_PRIORITY").</param>
        /// <returns>The reference set entity, or null if not found.</returns>
        Task<RefSet?> GetByCodeAsync(string code);

        /// <summary>
        /// Retrieves a reference set by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the reference set.</param>
        /// <returns>The reference set entity, or null if not found.</returns>
        Task<RefSet?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new reference set to the repository.
        /// </summary>
        /// <param name="refSet">The reference set entity to add.</param>
        /// <returns>The added reference set entity.</returns>
        Task<RefSet> AddAsync(RefSet refSet);

        /// <summary>
        /// Updates an existing reference set in the repository.
        /// </summary>
        /// <param name="refSet">The reference set entity with updated values.</param>
        Task UpdateAsync(RefSet refSet);
    }
}
