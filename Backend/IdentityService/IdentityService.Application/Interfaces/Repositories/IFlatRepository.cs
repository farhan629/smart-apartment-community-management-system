using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface for the repository managing flat/unit entities.
    /// </summary>
    public interface IFlatRepository
    {
        /// <summary>
        /// Retrieves a flat by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the flat.</param>
        /// <returns>The flat entity, or null if not found.</returns>
        Task<Flat?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a flat by its number and block name.
        /// </summary>
        /// <param name="number">The flat number.</param>
        /// <param name="block">The block name.</param>
        /// <returns>The flat entity, or null if not found.</returns>
        Task<Flat?> GetByNumberAndBlockAsync(string number, string block);

        /// <summary>
        /// Checks if there are any flats in the repository.
        /// </summary>
        /// <returns>True if any flats exist, otherwise false.</returns>
        Task<bool> AnyAsync();

        /// <summary>
        /// Adds a new flat to the repository.
        /// </summary>
        /// <param name="flat">The flat entity to add.</param>
        /// <returns>The added flat entity.</returns>
        Task<Flat> AddAsync(Flat flat);

        /// <summary>
        /// Adds a collection of flats to the repository.
        /// </summary>
        /// <param name="flats">The collection of flat entities to add.</param>
        Task AddRangeAsync(IEnumerable<Flat> flats);

        /// <summary>
        /// Updates an existing flat's details.
        /// </summary>
        /// <param name="flat">The flat entity to update.</param>
        Task UpdateAsync(Flat flat);

        /// <summary>
        /// Deletes a flat by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the flat to delete.</param>
        /// <returns>True if deleted successfully, otherwise false.</returns>
        Task<bool> DeleteAsync(Guid id);

        Task<(int TotalCount, List<Flat> Items)> GetPagedSortedByAvailabilityAsync(
            int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<Flat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    }
}
