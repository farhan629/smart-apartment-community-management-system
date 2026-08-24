using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface for the repository managing reference terms.
    /// </summary>
    public interface IRefTermRepository
    {
        /// <summary>
        /// Retrieves a reference term by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the reference term.</param>
        /// <returns>The reference term entity, or null if not found.</returns>
        Task<RefTerm?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a reference term by its code.
        /// </summary>
        /// <param name="code">The specific term code (e.g. "URGENT").</param>
        Task<RefTerm?> GetByCodeAsync(string code);

        /// <summary>
        /// Retrieves a reference term by its code and reference set identifier.
        /// </summary>
        /// <param name="code">The specific term code (e.g. "URGENT").</param>
        /// <param name="refSetId">The unique identifier of the reference set.</param>
        /// <returns>The reference term entity, or null if not found.</returns>
        Task<RefTerm?> GetByCodeAndSetIdAsync(string code, Guid refSetId);

        /// <summary>
        /// Retrieves all reference terms associated with a reference set identifier.
        /// </summary>
        /// <param name="refSetId">The unique identifier of the reference set.</param>
        /// <returns>A collection of reference terms.</returns>
        Task<IEnumerable<RefTerm>> GetByRefSetIdAsync(Guid refSetId);

        /// <summary>
        /// Adds a new reference term to the repository.
        /// </summary>
        /// <param name="refTerm">The reference term entity to add.</param>
        /// <returns>The added reference term entity.</returns>
        Task<RefTerm> AddAsync(RefTerm refTerm);

        /// <summary>
        /// Updates an existing reference term in the repository.
        /// </summary>
        /// <param name="refTerm">The reference term entity with updated values.</param>
        Task UpdateAsync(RefTerm refTerm);

        /// <summary>
        /// Deletes a reference term by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the reference term to delete.</param>
        Task DeleteAsync(Guid id);
    }
}
