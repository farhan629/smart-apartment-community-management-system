using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Domain.Entities;
using Shared.SharedLibrary.Services;

namespace ResidentVisitorService.Infrastructure.Persistence.DBContext;

/// <summary>
/// Represents the database context for the Resident Visitor Service, handling entity configurations and relationships.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService
    )
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets or sets the RefSets table.
    /// </summary>
    public DbSet<RefSet> RefSets { get; set; }

    /// <summary>
    /// Gets or sets the RefTerms table.
    /// </summary>
    public DbSet<RefTerm> RefTerms { get; set; }

    /// <summary>
    /// Gets or sets the Visitors table.
    /// </summary>
    public DbSet<Visitor> Visitors { get; set; }

    /// <summary>
    /// Gets or sets the Visits table.
    /// </summary>
    public DbSet<Visit> Visits { get; set; }

    /// <summary>
    /// Gets or sets the VisitQrTokens table.
    /// </summary>
    public DbSet<VisitQrToken> VisitQrTokens { get; set; }

    /// <summary>
    /// Configures the entity relationships, schema, and naming conventions.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(ResidentVisitorConstants.Database.SchemaName);

        modelBuilder.Entity<RefSet>(entity =>
        {
            entity.ToTable(ResidentVisitorConstants.TableNames.RefSets);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);

            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<RefTerm>(entity =>
        {
            entity.ToTable(ResidentVisitorConstants.TableNames.RefTerms);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);

            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);

            entity.HasIndex(e => new { e.RefSetId, e.Code }).IsUnique();

            entity
                .HasOne(e => e.RefSet)
                .WithMany(r => r.RefTerms)
                .HasForeignKey(e => e.RefSetId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.ToTable(ResidentVisitorConstants.TableNames.Visitors);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);

            entity.Property(e => e.Email).HasMaxLength(200);

            entity.Property(e => e.PhotoUrl).HasMaxLength(500);

            entity.HasIndex(e => e.PhoneNumber).IsUnique();

            entity
                .HasOne(e => e.VisitorType)
                .WithMany(rt => rt.Visitors)
                .HasForeignKey(e => e.VisitorTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.ToTable(ResidentVisitorConstants.TableNames.Visits);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.RejectionReason).HasMaxLength(500);

            entity.HasIndex(e => e.HostUserId);
            entity.HasIndex(e => e.FlatId);
            entity.HasIndex(e => e.VisitorId);
            entity.HasIndex(e => e.StatusId);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
            entity.HasIndex(e => e.ApprovedBy);

            entity
                .HasOne(e => e.Visitor)
                .WithMany(v => v.Visits)
                .HasForeignKey(e => e.VisitorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.PurposeType)
                .WithMany(rt => rt.Visits)
                .HasForeignKey(e => e.PurposeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VisitQrToken>(entity =>
        {
            entity.ToTable(ResidentVisitorConstants.TableNames.VisitQrTokens);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);

            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.VisitId).IsUnique();

            entity
                .HasOne(e => e.Visit)
                .WithOne(v => v.VisitQrToken)
                .HasForeignKey<VisitQrToken>(e => e.VisitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

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
    private string ToSnakeCase(string name)
    {
        return string.Concat(
                name.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())
            )
            .ToLower();
    }

    /// <summary>
    /// Saves changes asynchronously with audit trail for CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, and IsActive fields.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            var createdAt = entity
                .GetType()
                .GetProperty(ResidentVisitorConstants.AuditFields.CreatedAt);
            var createdBy = entity
                .GetType()
                .GetProperty(ResidentVisitorConstants.AuditFields.CreatedBy);
            var updatedAt = entity
                .GetType()
                .GetProperty(ResidentVisitorConstants.AuditFields.UpdatedAt);
            var updatedBy = entity
                .GetType()
                .GetProperty(ResidentVisitorConstants.AuditFields.UpdatedBy);
            var isActive = entity
                .GetType()
                .GetProperty(ResidentVisitorConstants.AuditFields.IsActive);

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
