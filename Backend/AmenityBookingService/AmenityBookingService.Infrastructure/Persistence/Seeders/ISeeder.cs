namespace AmenityBookingService.Infrastructure.Persistence.Seeders;

/// <summary>
/// Defines a contract for database seeders that populate initial data.
/// </summary>
public interface ISeeder
{
    /// <summary>
    /// Seeds data into the database.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous seeding operation.
    /// </returns>
    Task SeedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the execution order of the seeder.
    /// Seeders with lower order values are executed first.
    /// </summary>
    int Order { get; }
}
