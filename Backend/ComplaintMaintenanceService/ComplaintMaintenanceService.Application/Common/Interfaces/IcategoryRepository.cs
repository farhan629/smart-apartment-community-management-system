using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface for the repository managing work categories.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Retrieves a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <returns>The category entity, or null if not found.</returns>
        Task<Category?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a category by its unique name.
        /// </summary>
        /// <param name="name">The name of the category.</param>
        /// <returns>The category entity, or null if not found.</returns>
        Task<Category?> GetByNameAsync(string name);

        /// <summary>
        /// Checks if there are any categories in the repository.
        /// </summary>
        /// <returns>True if any categories exist, otherwise false.</returns>
        Task<bool> AnyAsync();

        /// <summary>
        /// Adds a new category to the repository.
        /// </summary>
        /// <param name="category">The category entity to add.</param>
        /// <returns>The added category entity.</returns>
        Task<Category> AddAsync(Category category);

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A collection of all category entities.</returns>
        Task<IEnumerable<Category>> GetAllAsync();

        /// <summary>
        /// Updates an existing category in the repository.
        /// </summary>
        /// <param name="category">The category entity with updated values.</param>
        Task UpdateAsync(Category category);
    }
}
