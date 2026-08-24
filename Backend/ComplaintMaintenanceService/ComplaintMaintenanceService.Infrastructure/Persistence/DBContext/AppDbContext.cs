using System.Linq;
using ComplaintMaintenanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shared.SharedLibrary.Services;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;

/// <summary>
/// Represents the database context for the Complaint Maintenance Service, handling entity configurations and relationships.
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
    /// Gets or sets the RefSets table.
    /// </summary>
    public DbSet<RefSet> RefSets { get; set; }

    /// <summary>
    /// Gets or sets the RefTerms table.
    /// </summary>
    public DbSet<RefTerm> RefTerms { get; set; }

    /// <summary>
    /// Gets or sets the Categories table.
    /// </summary>
    public DbSet<Category> Categories { get; set; }

    /// <summary>
    /// Gets or sets the Staff table.
    /// </summary>
    public DbSet<Staff> Staff { get; set; }

    /// <summary>
    /// Gets or sets the StaffAvailabilities table.
    /// </summary>
    public DbSet<StaffAvailability> StaffAvailabilities { get; set; }

    /// <summary>
    /// Gets or sets the Complaints table.
    /// </summary>
    public DbSet<Complaint> Complaints { get; set; }

    /// <summary>
    /// Gets or sets the ComplaintAssignments table.
    /// </summary>
    public DbSet<ComplaintAssignment> ComplaintAssignments { get; set; }

    /// <summary>
    /// Gets or sets the ComplaintComments table.
    /// </summary>
    public DbSet<ComplaintComment> ComplaintComments { get; set; }

    /// <summary>
    /// Gets or sets the ComplaintProgressLogs table.
    /// </summary>
    public DbSet<ComplaintProgressLog> ComplaintProgressLogs { get; set; }

    /// <summary>
    /// Gets or sets the ComplaintEscalations table.
    /// </summary>
    public DbSet<ComplaintEscalation> ComplaintEscalations { get; set; }

    /// <summary>
    /// Gets or sets the AutoAssignmentRules table.
    /// </summary>
    public DbSet<AutoAssignmentRule> AutoAssignmentRules { get; set; }

    /// <summary>
    /// Configures the entity relationships, schema, and naming conventions.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("DB_TEAM_C_complaint");

        // ── RefSet ──────────────────────────────────────────────────────────
        modelBuilder.Entity<RefSet>(entity =>
        {
            entity.ToTable("ref_sets");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);

            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.Code).IsUnique();
        });

        // ── RefTerm ─────────────────────────────────────────────────────────
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

        // ── Category ────────────────────────────────────────────────────────
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

            entity.Property(e => e.Description).HasMaxLength(500);

            entity.Property(e => e.Img).HasMaxLength(500);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // ── Staff ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("staff");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Description).HasMaxLength(500);

            entity.Property(e => e.Details).HasMaxLength(1000);

            entity.HasIndex(e => e.UserId).IsUnique();

            entity
                .HasOne(e => e.Category)
                .WithMany(c => c.Staff)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── StaffAvailability ───────────────────────────────────────────────
        modelBuilder.Entity<StaffAvailability>(entity =>
        {
            entity.ToTable("staff_availabilities");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.IsBooked).HasDefaultValue(false);
            entity.Property(e => e.IsCancelled).HasDefaultValue(false);

            entity
                .HasOne(e => e.Staff)
                .WithMany(s => s.StaffAvailabilities)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(e => e.Complaint)
                .WithMany()
                .HasForeignKey(e => e.ComplaintId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ── Complaint ───────────────────────────────────────────────────────
        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.ToTable("complaints");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);

            entity.Property(e => e.CancellationReason).HasMaxLength(500);

            entity
                .HasOne(e => e.Category)
                .WithMany(c => c.Complaints)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.ComplaintType)
                .WithMany(r => r.Complaints)
                .HasForeignKey(e => e.ComplaintTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Priority)
                .WithMany()
                .HasForeignKey(e => e.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.ScheduledSlot)
                .WithMany()
                .HasForeignKey(e => e.ScheduledSlotId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ── ComplaintAssignment ─────────────────────────────────────────────
        modelBuilder.Entity<ComplaintAssignment>(entity =>
        {
            entity.ToTable("complaint_assignments");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DenialReason).HasMaxLength(500);

            entity
                .HasOne(e => e.Complaint)
                .WithMany(c => c.ComplaintAssignments)
                .HasForeignKey(e => e.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(e => e.Staff)
                .WithMany(s => s.ComplaintAssignments)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Status)
                .WithMany(r => r.ComplaintAssignments)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ComplaintComment ────────────────────────────────────────────────
        modelBuilder.Entity<ComplaintComment>(entity =>
        {
            entity.ToTable("complaint_comments");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CommentText).IsRequired().HasMaxLength(2000);

            entity
                .HasOne(e => e.Complaint)
                .WithMany(c => c.ComplaintComments)
                .HasForeignKey(e => e.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ComplaintProgressLog ────────────────────────────────────────────
        modelBuilder.Entity<ComplaintProgressLog>(entity =>
        {
            entity.ToTable("complaint_progress_logs");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Remarks).HasMaxLength(1000);

            entity
                .HasOne(e => e.Complaint)
                .WithMany(c => c.ComplaintProgressLogs)
                .HasForeignKey(e => e.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(e => e.Status)
                .WithMany(r => r.ComplaintProgressLogs)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ComplaintEscalation ─────────────────────────────────────────────
        modelBuilder.Entity<ComplaintEscalation>(entity =>
        {
            entity.ToTable("complaint_escalations");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.EscalationReason).IsRequired().HasMaxLength(1000);

            entity
                .HasOne(e => e.Complaint)
                .WithMany(c => c.ComplaintEscalations)
                .HasForeignKey(e => e.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── AutoAssignmentRule ──────────────────────────────────────────────
        modelBuilder.Entity<AutoAssignmentRule>(entity =>
        {
            entity.ToTable("auto_assignment_rules");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AllowResidentTimePick).HasDefaultValue(false);

            entity.HasIndex(e => new { e.CategoryId, e.PriorityId }).IsUnique();

            entity
                .HasOne(e => e.Category)
                .WithMany(c => c.AutoAssignmentRules)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Priority)
                .WithMany(r => r.AutoAssignmentRules)
                .HasForeignKey(e => e.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Staff)
                .WithMany(s => s.AutoAssignmentRules)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.FallbackStaff)
                .WithMany()
                .HasForeignKey(e => e.FallbackStaffId)
                .OnDelete(DeleteBehavior.SetNull);
        });

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
