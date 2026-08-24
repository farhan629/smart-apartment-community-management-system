using System.Linq;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shared.SharedLibrary.Services;

namespace IdentityService.Infrastructure.Persistence.DBContext;

/// <summary>
/// Represents the database context for the Identity Service, handling entity configurations and relationships.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

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
    /// Gets or sets the Users table.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the RefSets table.
    /// </summary>
    public DbSet<RefSet> RefSets { get; set; }

    /// <summary>
    /// Gets or sets the RefTerms table.
    /// </summary>
    public DbSet<RefTerm> RefTerms { get; set; }

    /// <summary>
    /// Gets or sets the RolePolicies table.
    /// </summary>
    public DbSet<RolePolicy> RolePolicies { get; set; }

    /// <summary>
    /// Gets or sets the UserPasswordSecurities table.
    /// </summary>
    public DbSet<UserPasswordSecurity> UserPasswordSecurities { get; set; }

    /// <summary>
    /// Gets or sets the UserPolicies table.
    /// </summary>
    public DbSet<UserPolicy> UserPolicies { get; set; }

    /// <summary>
    /// Gets or sets the Flats table.
    /// </summary>
    public DbSet<Flat> Flats { get; set; }

    /// <summary>
    /// Gets or sets the FlatOccupancies table.
    /// </summary>
    public DbSet<FlatOccupancy> FlatOccupancies { get; set; }


    /// <summary>
    /// Gets or sets the RefreshTokens.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <summary>
    /// Configures the entity relationships, schema, and naming conventions.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("DB_TEAM_C_identity");

        modelBuilder.Entity<RefSet>(entity =>
        {
            entity.ToTable("ref_sets");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);

            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<RefTerm>(entity =>
        {
            entity.ToTable("ref_terms");
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

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);

            entity.Property(e => e.PhoneNo).IsRequired().HasMaxLength(20);

            entity.Property(e => e.PhotoUrl).HasMaxLength(500);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.PhoneNo).IsUnique();

            entity
                .HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.PasswordSecurity)
                .WithOne(p => p.User)
                .HasForeignKey<UserPasswordSecurity>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserPasswordSecurity>(entity =>
        {
            entity.ToTable("user_password_securities");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);

            entity.HasIndex(e => e.UserId).IsUnique();

            entity
                .HasOne(e => e.User)
                .WithOne(u => u.PasswordSecurity)
                .HasForeignKey<UserPasswordSecurity>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<RolePolicy>(entity =>
        {
            entity.ToTable("role_policies");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PermissionCode).IsRequired().HasMaxLength(100);

            entity.Property(e => e.Description).HasMaxLength(500);

            entity.Property(e => e.IsAllowed).IsRequired().HasDefaultValue(true);

            entity.HasIndex(e => new { e.RoleId, e.PermissionCode }).IsUnique();

            entity
                .HasOne(e => e.Role)
                .WithMany(r => r.RolePolicies)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserPolicy>(entity =>
        {
            entity.ToTable("user_policies");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PermissionCode).IsRequired().HasMaxLength(100);

            entity.Property(e => e.IsAllowed).IsRequired().HasDefaultValue(true);

            entity.HasIndex(e => new { e.UserId, e.PermissionCode }).IsUnique();

            entity
                .HasOne(e => e.User)
                .WithMany(u => u.UserPolicies)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Flat>(entity =>
        {
            entity.ToTable("flats");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Number).IsRequired().HasMaxLength(50);

            entity.Property(e => e.Block).IsRequired().HasMaxLength(50);

            entity.Property(e => e.Floor).IsRequired();

            entity.HasIndex(e => new { e.Block, e.Number }).IsUnique();
        });

        modelBuilder.Entity<FlatOccupancy>(entity =>
        {
            entity.ToTable("flat_occupancies");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.IsApproved).IsRequired().HasDefaultValue(false);

            entity.HasIndex(e => new { e.UserId, e.FlatId }).IsUnique();

            entity
                .HasOne(e => e.Flat)
                .WithMany(f => f.FlatOccupancies)
                .HasForeignKey(e => e.FlatId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(e => e.User)
                .WithMany(u => u.FlatOccupancies)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.ResidentType)
                .WithMany(rt => rt.FlatOccupancies)
                .HasForeignKey(e => e.ResidentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
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
                if (property.Name == "DateCreated")
                {
                    property.SetColumnName("created_at");
                }
                else if (property.Name == "DateUpdated")
                {
                    property.SetColumnName("updated_at");
                }
                else
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
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
