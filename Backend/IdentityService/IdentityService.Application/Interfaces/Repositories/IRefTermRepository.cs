using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories
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
        /// Retrieves a reference term by its term value and set name.
        /// </summary>
        /// <param name="termValue">The specific term value (e.g. "Admin", "Owner").</param>
        /// <param name="setName">The name of the reference set.</param>
        /// <returns>The reference term entity, or null if not found.</returns>
        Task<RefTerm?> GetByTermValueAsync(string termValue, string setName);

        /// <summary>
        /// Retrieves a reference term by its term value and reference set identifier.
        /// </summary>
        /// <param name="termValue">The specific term value.</param>
        /// <param name="refSetId">The unique identifier of the reference set.</param>
        /// <returns>The reference term entity, or null if not found.</returns>
        Task<RefTerm?> GetByTermValueAndSetIdAsync(string termValue, Guid refSetId);

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
        /// Deletes a reference term by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the reference term to delete.</param>
        Task DeleteAsync(Guid id);
    }
}
