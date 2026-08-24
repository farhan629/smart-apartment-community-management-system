using System;
using System.Threading.Tasks;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface for the repository managing flat occupancy requests and approvals.
    /// </summary>
    public interface IFlatOccupancyRepository
    {
        /// <summary>
        /// Checks if there is any pending occupancy request for a specific flat.
        /// </summary>
        /// <param name="flatId">The unique identifier of the flat.</param>
        /// <returns>True if a pending request exists, otherwise false.</returns>
        Task<bool> HasPendingRequestAsync(Guid flatId);

        /// <summary>
        /// Adds a new flat occupancy request to the repository.
        /// </summary>
        /// <param name="occupancy">The flat occupancy entity to add.</param>
        /// <returns>The added flat occupancy entity.</returns>
        Task<FlatOccupancy> AddAsync(FlatOccupancy occupancy);

        /// <summary>
        /// Retrieves a flat occupancy request by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the flat occupancy record.</param>
        /// <returns>The flat occupancy entity, or null if not found.</returns>
        Task<FlatOccupancy?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a paginated, optionally filtered list of occupancy (approval) records.
        /// </summary>
        /// <param name="status">"pending" (IsApproved=false), "approved" (IsApproved=true), or null for all.</param>
        /// <param name="userId">Optional filter to a specific user's occupancy requests.</param>
        Task<(int Total, IEnumerable<FlatOccupancy> Items)> GetAllAsync(
            int page,
            int limit,
            string? status,
            Guid? userId);

        /// <summary>
        /// Updates an existing occupancy record (e.g. after approve/reject).
        /// </summary>
        Task UpdateAsync(FlatOccupancy occupancy);

        /// <summary>
        /// Retrieves any active (approved or pending) occupancy for a flat with a specific resident type.
        /// </summary>
        Task<FlatOccupancy?> GetActiveOccupancyByFlatAndRoleAsync(Guid flatId, Guid residentTypeId);


        Task<Guid?> getUserIdFlat(Guid userId);
    }
}
