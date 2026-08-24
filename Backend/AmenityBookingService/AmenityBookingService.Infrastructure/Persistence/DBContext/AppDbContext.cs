using System.Linq;
using AmenityBookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Infrastructure.Persistence.DBContext;

/// <summary>
/// Represents the database context for the Amenity Booking Service, handling entity configurations and relationships.
/// </summary>
public class AppDbContext : DbContext
{
    public readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService
    )
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets or sets the Amenities table.
    /// </summary>
    public DbSet<Amenity> Amenities { get; set; }

    /// <summary>
    /// Gets or sets the AmenitySlots table.
    /// </summary>
    public DbSet<AmenitySlot> AmenitySlots { get; set; }

    /// <summary>
    /// Gets or sets the AmenityBookings table.
    /// </summary>
    public DbSet<AmenityBooking> AmenityBookings { get; set; }

    /// <summary>
    /// Gets or sets the RefSets table.
    /// </summary>
    public DbSet<RefSet> RefSets { get; set; }

    /// <summary>
    /// Gets or sets the RefTerms table.
    /// </summary>
    public DbSet<RefTerm> RefTerms { get; set; }

    /// <summary>
    /// Configures the entity relationships, schema, and naming conventions.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("DB_TEAM_C_amenity");

        // Amenity configurations
        modelBuilder
            .Entity<Amenity>()
            .HasOne(a => a.SlotType)
            .WithMany(rt => rt.Amenities)
            .HasForeignKey(a => a.SlotTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Amenity>()
            .HasOne(a => a.Status)
            .WithMany()
            .HasForeignKey(a => a.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // AmenitySlot configurations
        modelBuilder
            .Entity<AmenitySlot>()
            .HasOne(ams => ams.Amenity)
            .WithMany(a => a.AmenitySlots)
            .HasForeignKey(ams => ams.AmenityId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<AmenitySlot>()
            .HasIndex(ams => new
            {
                ams.AmenityId,
                ams.SlotDate,
                ams.StartTime,
            })
            .IsUnique();

        // AmenityBooking configurations
        modelBuilder
            .Entity<AmenityBooking>()
            .HasOne(ab => ab.AmenitySlot)
            .WithMany(ams => ams.AmenityBookings)
            .HasForeignKey(ab => ab.AmenitySlotId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AmenityBooking>()
            .HasOne(ab => ab.BookingStatus)
            .WithMany(rt => rt.AmenityBookings)
            .HasForeignKey(ab => ab.BookingStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AmenityBooking>()
            .HasIndex(ab => new { ab.UserId, ab.AmenitySlotId })
            .IsUnique()
            .HasFilter("is_active = true");

        // RefTerm configurations
        modelBuilder
            .Entity<RefTerm>()
            .HasOne(rt => rt.RefSet)
            .WithMany(rs => rs.RefTerms)
            .HasForeignKey(rt => rt.RefSetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefTerm>().HasIndex(rt => new { rt.RefSetId, rt.Code }).IsUnique();

        // Apply snake_case naming convention to all tables and columns
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName))
                {
                    key.SetName(ToSnakeCase(keyName));
                }
            }

            foreach (var fk in entity.GetForeignKeys())
            {
                var constraintName = fk.GetConstraintName();
                if (!string.IsNullOrEmpty(constraintName))
                {
                    fk.SetConstraintName(ToSnakeCase(constraintName));
                }
            }

            foreach (var index in entity.GetIndexes())
            {
                var databaseName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(databaseName))
                {
                    index.SetDatabaseName(ToSnakeCase(databaseName));
                }
            }
        }
    }

    /// <summary>
    /// Converts a given string to snake_case format.
    /// </summary>
    /// <param name="name">The input string.</param>
    /// <returns>The snake_case formatted string.</returns>
    private string ToSnakeCase(string name)
    {
        return string.Concat(
                name.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())
            )
            .ToLower();
    }

    /// <summary>
    /// Saves changes asynchronously with audit trail for CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, and IsDeleted fields.
    /// </summary>
    /// <param name="userId">The ID of the user performing the operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            var createdAt = entity.GetType().GetProperty("CreatedAt");
            var createdBy = entity.GetType().GetProperty("CreatedBy");
            var updatedAt = entity.GetType().GetProperty("UpdatedAt");
            var updatedBy = entity.GetType().GetProperty("UpdatedBy");
            var isActive = entity.GetType().GetProperty("IsActive");

            if (entry.State == EntityState.Added)
            {
                createdAt?.SetValue(entity, DateTime.UtcNow);
                createdBy?.SetValue(entity, userId);
                isActive?.SetValue(entity, true);
            }
            else if (entry.State == EntityState.Modified)
            {
                updatedAt?.SetValue(entity, DateTime.UtcNow);
                updatedBy?.SetValue(entity, userId);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
