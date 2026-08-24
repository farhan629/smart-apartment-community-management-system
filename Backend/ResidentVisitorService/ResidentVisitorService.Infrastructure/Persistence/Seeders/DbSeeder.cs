using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Domain.Entities;
using ResidentVisitorService.Infrastructure.Persistence.DBContext;

namespace ResidentVisitorService.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeds reference data (RefSets and RefTerms) from embedded CSV files on application startup.
/// Deserializes directly into entity types (no intermediate DTOs) since CSV column
/// names match the entity property names that are seedable.
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

    /// <summary>
    /// Runs all seed operations. Applies pending migrations, then seeds ref data if empty.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
            await SeedRefSetsAsync();
            await SeedRefTermsAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    /// <summary>Seeds RefSets from the embedded ref_sets.csv — adds missing rows and updates changed ones.</summary>
    private async Task SeedRefSetsAsync()
    {
        var records = ReadCsv<RefSet>(ResidentVisitorConstants.Seeder.RefSetsCsvFile);

        var existingRefSets = await _context.RefSets.ToDictionaryAsync(rs => rs.Id);

        var newRefSets = new List<RefSet>();
        var updatedCount = 0;

        foreach (var r in records)
        {
            if (existingRefSets.TryGetValue(r.Id, out var existing))
            {
                if (existing.Code != r.Code || existing.Description != r.Description)
                {
                    existing.Code = r.Code;
                    existing.Description = r.Description;
                    existing.UpdatedAt = DateTime.UtcNow;
                    updatedCount++;
                }

                continue;
            }

            r.IsActive = true;
            r.CreatedAt = DateTime.UtcNow;
            r.UpdatedAt = DateTime.UtcNow;
            newRefSets.Add(r);
        }

        if (newRefSets.Count == 0 && updatedCount == 0)
        {
            _logger.LogInformation("RefSets already up to date — nothing to seed");
            return;
        }

        if (newRefSets.Count > 0)
        {
            await _context.RefSets.AddRangeAsync(newRefSets);
            await _context.Database.ExecuteSqlRawAsync(ResidentVisitorConstants.Seeder.FlushSql);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Seeded {NewCount} new RefSets, updated {UpdatedCount} existing RefSets",
            newRefSets.Count,
            updatedCount
        );
    }

    /// <summary>Seeds RefTerms from the embedded ref_terms.csv — adds missing rows and updates changed ones.</summary>
    private async Task SeedRefTermsAsync()
    {
        var records = ReadCsv<RefTerm>(ResidentVisitorConstants.Seeder.RefTermsCsvFile);

        var existingRefTerms = await _context.RefTerms.ToDictionaryAsync(rt => rt.Id);

        var newRefTerms = new List<RefTerm>();
        var updatedCount = 0;

        foreach (var r in records)
        {
            if (existingRefTerms.TryGetValue(r.Id, out var existing))
            {
                if (
                    existing.RefSetId != r.RefSetId
                    || existing.Code != r.Code
                    || existing.DisplayName != r.DisplayName
                )
                {
                    existing.RefSetId = r.RefSetId;
                    existing.Code = r.Code;
                    existing.DisplayName = r.DisplayName;
                    existing.UpdatedAt = DateTime.UtcNow;
                    updatedCount++;
                }

                continue;
            }

            r.IsActive = true;
            r.CreatedAt = DateTime.UtcNow;
            r.UpdatedAt = DateTime.UtcNow;
            newRefTerms.Add(r);
        }

        if (newRefTerms.Count == 0 && updatedCount == 0)
        {
            _logger.LogInformation("RefTerms already up to date — nothing to seed");
            return;
        }

        if (newRefTerms.Count > 0)
        {
            await _context.RefTerms.AddRangeAsync(newRefTerms);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Seeded {NewCount} new RefTerms, updated {UpdatedCount} existing RefTerms",
            newRefTerms.Count,
            updatedCount
        );
    }

    /// <summary>Reads an embedded CSV resource by filename and deserializes directly into <typeparamref name="T"/>.</summary>
    private static List<T> ReadCsv<T>(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName =
            assembly
                .GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
            ?? throw new FileNotFoundException(
                string.Format(ResidentVisitorConstants.Seeder.EmbeddedResourceNotFound, fileName)
            );

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(
            reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                HeaderValidated = null,
                MissingFieldFound = null,
            }
        );

        return csv.GetRecords<T>().ToList();
    }
}
