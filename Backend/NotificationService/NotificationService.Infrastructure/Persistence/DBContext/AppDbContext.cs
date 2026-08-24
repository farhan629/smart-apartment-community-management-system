using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NotificationService.Application.Constants;
using NotificationService.Domain.Entities;
using Shared.SharedLibrary.Services;

namespace NotificationService.Infrastructure.Persistence.DBContext;

/// <summary>
/// EF Core database context for the <c>NotificationService</c>, registering all entity sets,
/// applying the <see cref="NotificationConstants.SCHEMA_NAME"/> default schema, enforcing a
/// global snake_case naming convention across tables, columns, keys, foreign keys, and indexes,
/// and providing automatic audit-field population on every save.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of <see cref="AppDbContext"/>.
    /// </summary>
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService
    )
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>Gets or sets the <see cref="RefSet"/> entity set, mapped to the <c>ref_sets</c> table.</summary>
    public DbSet<RefSet> RefSets { get; set; }

    /// <summary>Gets or sets the <see cref="RefTerm"/> entity set, mapped to the <c>ref_terms</c> table.</summary>
    public DbSet<RefTerm> RefTerms { get; set; }

    /// <summary>Gets or sets the <see cref="EmailTemplate"/> entity set, mapped to the <c>email_templates</c> table.</summary>
    public DbSet<EmailTemplate> EmailTemplates { get; set; }

    /// <summary>Gets or sets the <see cref="EmailLog"/> entity set, mapped to the <c>email_logs</c> table.</summary>
    public DbSet<EmailLog> EmailLogs { get; set; }

    /// <summary>Gets or sets the <see cref="NotificationTemplate"/> entity set, mapped to the <c>notification_templates</c> table.</summary>
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    /// <summary>Gets or sets the <see cref="Notification"/> entity set, mapped to the <c>notifications</c> table.</summary>
    public DbSet<Notification> Notifications { get; set; }

    /// <summary>
    /// Applies all entity configurations, sets the default schema, and enforces a global
    /// snake_case naming convention across every table, column, key, foreign key, and index
    /// in the model.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(NotificationConstants.SCHEMA_NAME);

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

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.ToTable("email_templates");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmailType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.BodyTemplate).IsRequired().HasColumnType("text");
            entity.HasIndex(e => e.EmailType).IsUnique();
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.ToTable("email_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Body).IsRequired().HasColumnType("text");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.EmailAddress);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.SentAt);
            entity
                .HasOne(e => e.Template)
                .WithMany(t => t.EmailLogs)
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.ToTable("notification_templates");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NotificationType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.MessageTemplate).IsRequired().HasColumnType("text");
            entity.HasIndex(e => e.NotificationType).IsUnique();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasColumnType("text");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NotificationType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsRead).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.IsReminderSent).IsRequired().HasDefaultValue(false);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.VisitId);
            entity.HasIndex(e => e.ComplaintId);
            entity.HasIndex(e => e.AmenityBookingId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.NotificationType);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.ScheduledFor);
            entity.HasIndex(e => e.SentAt);
            entity
                .HasOne(e => e.Template)
                .WithMany(t => t.Notifications)
                .HasForeignKey(e => e.TemplateId)
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
    /// Converts a PascalCase or camelCase identifier to snake_case by inserting an underscore
    /// before each uppercase character (except the first) and lowercasing the result.
    /// </summary>
    private string ToSnakeCase(string name)
    {
        return string.Concat(
                name.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())
            )
            .ToLower();
    }

    /// <summary>
    /// Persists all pending changes to the database, automatically populating audit fields
    /// on every tracked entity before the underlying save is executed.
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

            foreach (var prop in entity.GetType().GetProperties())
            {
                if (
                    prop.PropertyType == typeof(DateTime)
                    && prop.GetValue(entity) is DateTime dt
                    && dt.Kind != DateTimeKind.Utc
                )
                {
                    prop.SetValue(entity, DateTime.SpecifyKind(dt, DateTimeKind.Utc));
                }
                else if (prop.PropertyType == typeof(DateTime?))
                {
                    var val = (DateTime?)prop.GetValue(entity);
                    if (val.HasValue && val.Value.Kind != DateTimeKind.Utc)
                        prop.SetValue(
                            entity,
                            (DateTime?)DateTime.SpecifyKind(val.Value, DateTimeKind.Utc)
                        );
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
