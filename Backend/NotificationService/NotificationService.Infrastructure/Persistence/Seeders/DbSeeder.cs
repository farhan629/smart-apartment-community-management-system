using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Constants;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence.DBContext;

namespace NotificationService.Infrastructure.Persistence.Seeder;

/// <summary>
/// Seeds reference data (notification types, email templates, notification templates)
/// from CSV files embedded as resources in this assembly, on application startup.
/// Deserializes directly into entity types (no intermediate DTOs) since CSV column
/// names match the entity property names that are seedable.
/// Adds missing rows and updates changed ones (upsert).
/// </summary>
public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(AppDbContext context, ILogger<DbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("=== NotificationService database seeding started ===");

        await _context.Database.MigrateAsync();

        await SeedNotificationTypesAsync();
        await SeedEmailTemplatesAsync();
        await SeedNotificationTemplatesAsync();

        _logger.LogInformation("=== NotificationService database seeding completed ===");
    }

    /// <summary>Seeds notification type RefTerms from an embedded CSV — adds missing rows and updates changed ones.</summary>
    private async Task SeedNotificationTypesAsync()
    {
        var records = ReadEmbeddedCsv<RefTerm>(
            NotificationConstants.SeedData.NOTIFICATION_TYPES_RESOURCE_NAME
        );
        if (records is null)
            return;

        var refSet = await _context.RefSets.FirstOrDefaultAsync(r =>
            r.Code == NotificationConstants.RefSetCodes.NOTIFICATION_TYPE
        );

        if (refSet is null)
        {
            refSet = new RefSet
            {
                Code = NotificationConstants.RefSetCodes.NOTIFICATION_TYPE,
                Description = NotificationConstants.SeedData.NOTIFICATION_TYPE_DESCRIPTION,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RefTerms = new List<RefTerm>(),
            };
            _context.RefSets.Add(refSet);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created RefSet '{Code}'.", refSet.Code);
        }

        var existingTerms = await _context
            .RefTerms.Where(t => t.RefSetId == refSet.Id)
            .ToDictionaryAsync(t => t.Code);

        var newTerms = new List<RefTerm>();
        var updatedCount = 0;

        foreach (var r in records)
        {
            if (string.IsNullOrWhiteSpace(r.Code) || string.IsNullOrWhiteSpace(r.DisplayName))
                continue;

            if (existingTerms.TryGetValue(r.Code, out var existing))
            {
                if (existing.DisplayName != r.DisplayName)
                {
                    existing.DisplayName = r.DisplayName;
                    existing.UpdatedAt = DateTime.UtcNow;
                    updatedCount++;
                    _logger.LogInformation("Updated notification type '{Code}'.", r.Code);
                }

                continue;
            }

            r.RefSetId = refSet.Id;
            r.CreatedAt = DateTime.UtcNow;
            r.UpdatedAt = DateTime.UtcNow;
            newTerms.Add(r);
        }

        if (newTerms.Count > 0)
        {
            _context.RefTerms.AddRange(newTerms);
        }

        if (newTerms.Count > 0 || updatedCount > 0)
        {
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation(
            "Notification types seeding complete: {New} added, {Updated} updated.",
            newTerms.Count,
            updatedCount
        );
    }

    /// <summary>Seeds email templates from an embedded CSV — adds missing rows and updates changed ones.</summary>
    private async Task SeedEmailTemplatesAsync()
    {
        var records = ReadEmbeddedCsv<EmailTemplate>(
            NotificationConstants.SeedData.EMAIL_TEMPLATES_RESOURCE_NAME
        );
        if (records is null)
            return;

        var existingTemplates = await _context.EmailTemplates.ToDictionaryAsync(t => t.EmailType);

        var newTemplates = new List<EmailTemplate>();
        var updatedCount = 0;

        foreach (var r in records)
        {
            if (string.IsNullOrWhiteSpace(r.EmailType))
                continue;

            r.Subject ??= string.Empty;
            r.BodyTemplate ??= string.Empty;

            if (existingTemplates.TryGetValue(r.EmailType, out var existing))
            {
                if (existing.Subject != r.Subject || existing.BodyTemplate != r.BodyTemplate)
                {
                    existing.Subject = r.Subject;
                    existing.BodyTemplate = r.BodyTemplate;
                    existing.UpdatedAt = DateTime.UtcNow;
                    updatedCount++;
                    _logger.LogInformation("Updated email template '{EmailType}'.", r.EmailType);
                }

                continue;
            }

            r.CreatedAt = DateTime.UtcNow;
            r.UpdatedAt = DateTime.UtcNow;
            r.IsActive = true;
            newTemplates.Add(r);
        }

        if (newTemplates.Count > 0)
        {
            _context.EmailTemplates.AddRange(newTemplates);
        }

        if (newTemplates.Count > 0 || updatedCount > 0)
        {
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation(
            "Email templates seeding complete: {New} added, {Updated} updated.",
            newTemplates.Count,
            updatedCount
        );
    }

    /// <summary>Seeds notification templates from an embedded CSV — adds missing rows and updates changed ones.</summary>
    private async Task SeedNotificationTemplatesAsync()
    {
        var records = ReadEmbeddedCsv<NotificationTemplate>(
            NotificationConstants.SeedData.NOTIFICATION_TEMPLATES_RESOURCE_NAME
        );
        if (records is null)
            return;

        var existingTemplates = await _context.NotificationTemplates.ToDictionaryAsync(t =>
            t.NotificationType
        );

        var newTemplates = new List<NotificationTemplate>();
        var updatedCount = 0;

        foreach (var r in records)
        {
            if (string.IsNullOrWhiteSpace(r.NotificationType))
                continue;

            r.Title ??= string.Empty;
            r.MessageTemplate ??= string.Empty;

            if (existingTemplates.TryGetValue(r.NotificationType, out var existing))
            {
                if (existing.Title != r.Title || existing.MessageTemplate != r.MessageTemplate)
                {
                    existing.Title = r.Title;
                    existing.MessageTemplate = r.MessageTemplate;
                    existing.UpdatedAt = DateTime.UtcNow;
                    updatedCount++;
                    _logger.LogInformation(
                        "Updated notification template '{Type}'.",
                        r.NotificationType
                    );
                }

                continue;
            }

            r.CreatedAt = DateTime.UtcNow;
            r.UpdatedAt = DateTime.UtcNow;
            r.IsActive = true;
            newTemplates.Add(r);
        }

        if (newTemplates.Count > 0)
        {
            _context.NotificationTemplates.AddRange(newTemplates);
        }

        if (newTemplates.Count > 0 || updatedCount > 0)
        {
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation(
            "Notification templates seeding complete: {New} added, {Updated} updated.",
            newTemplates.Count,
            updatedCount
        );
    }

    /// <summary>
    /// Reads a CSV file embedded as a resource in this assembly and deserializes
    /// directly into the given entity type. Returns null (and logs a warning) if
    /// the resource isn't found, so the caller can skip that seed step instead of throwing.
    /// </summary>
    private List<T>? ReadEmbeddedCsv<T>(string resourceName)
    {
        var assembly = typeof(DbSeeder).Assembly;

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            _logger.LogWarning(
                "Embedded CSV resource '{ResourceName}' not found — skipping this seed step. "
                    + "If this is unexpected, check the exact name via assembly.GetManifestResourceNames().",
                resourceName
            );
            return null;
        }

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            HeaderValidated = null,
            MissingFieldFound = null,
        };

        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, csvConfig);

        return csv.GetRecords<T>().ToList();
    }
}
