using System.Globalization;
using AmenityBookingService.Domain.Entities;
using AmenityBookingService.Infrastructure.Persistence.DBContext;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;

namespace AmenityBookingService.Infrastructure.Persistence.Seeders;

public class ReferenceDataSeeder : ISeeder
{
    private readonly AppDbContext _context;
    private readonly string _refSetsFilePath;
    private readonly string _refTermsFilePath;

    public int Order => 1;

    public ReferenceDataSeeder(AppDbContext context)
    {
        _context = context;
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        _refSetsFilePath = Path.Combine(baseDir, "Persistence", "SeedData", "refsets.csv");
        _refTermsFilePath = Path.Combine(baseDir, "Persistence", "SeedData", "refterms.csv");
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedRefSetsAsync(cancellationToken);
        await SeedRefTermsAsync(cancellationToken);
    }

    private async Task SeedRefSetsAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_refSetsFilePath))
            return;

        using var reader = new StreamReader(_refSetsFilePath);
        using var csv = new CsvReader(
            reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
            }
        );

        csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(
            new TypeConverterOptions { DateTimeStyle = DateTimeStyles.AdjustToUniversal }
        );

        var csvRecords = csv.GetRecords<RefSet>().ToList();
        var existing = await _context.RefSets.ToDictionaryAsync(r => r.Id, cancellationToken);

        foreach (var record in csvRecords)
        {
            if (existing.TryGetValue(record.Id, out var entity))
            {
                if (
                    entity.Code != record.Code?.Trim()
                    || entity.Description != record.Description?.Trim()
                    || entity.IsActive != record.IsActive
                )
                {
                    entity.Code = record.Code?.Trim() ?? string.Empty;
                    entity.Description = record.Description?.Trim() ?? string.Empty;
                    entity.IsActive = record.IsActive;
                }
            }
            else
            {
                record.UpdatedAt = DateTime.UtcNow;
                _context.RefSets.Add(record);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedRefTermsAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_refTermsFilePath))
            return;

        using var reader = new StreamReader(_refTermsFilePath);
        using var csv = new CsvReader(
            reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
            }
        );

        csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(
            new TypeConverterOptions { DateTimeStyle = DateTimeStyles.AdjustToUniversal }
        );

        var csvRecords = csv.GetRecords<RefTerm>().ToList();
        var existing = await _context.RefTerms.ToDictionaryAsync(r => r.Id, cancellationToken);

        foreach (var record in csvRecords)
        {
            if (existing.TryGetValue(record.Id, out var entity))
            {
                if (
                    entity.RefSetId != record.RefSetId
                    || entity.Code != record.Code?.Trim()
                    || entity.DisplayName != record.DisplayName?.Trim()
                    || entity.IsActive != record.IsActive
                )
                {
                    entity.RefSetId = record.RefSetId;
                    entity.Code = record.Code?.Trim() ?? string.Empty;
                    entity.DisplayName = record.DisplayName?.Trim() ?? string.Empty;
                    entity.IsActive = record.IsActive;
                }
            }
            else
            {
                record.UpdatedAt = DateTime.UtcNow;
                _context.RefTerms.Add(record);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
