using AmenityBookingService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace AmenityBookingService.Infrastructure.Persistence.Seeders;

/// <summary>
/// Coordinates the execution of all registered database seeders.
/// </summary>
public class Seeder
{
    private readonly AppDbContext _context;
    private readonly IEnumerable<ISeeder> _seeders;

    /// <summary>
    /// Initializes a new instance of the <see cref="Seeder"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="seeders">
    /// The collection of registered seeders to execute.
    /// </param>
    public Seeder(AppDbContext context, IEnumerable<ISeeder> seeders)
    {
        _context = context;
        _seeders = seeders.OrderBy(s => s.Order);
    }

    /// <summary>
    /// Applies any pending database migrations and executes all registered seeders
    /// in their configured execution order.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous seeding operation.
    /// </returns>
    public async Task SeedAllAsync(CancellationToken cancellationToken = default)
    {
        if (!await _context.Database.CanConnectAsync(cancellationToken))
            await _context.Database.EnsureCreatedAsync(cancellationToken);

        foreach (var seeder in _seeders)
        {
            await seeder.SeedAsync(cancellationToken);
        }
    }
}
