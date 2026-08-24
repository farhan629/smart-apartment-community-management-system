using System.Globalization;
using System.Text;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Seeders
{
    /// <summary>
    /// Orchestrates and seeds the database from embedded CSV files (ref_sets, ref_terms, category).
    /// Existing rows (matched by Id) are updated if their values have changed, rather than skipped.
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly IRefSetRepository _refSetRepository;
        private readonly IRefTermRepository _refTermRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            IRefSetRepository refSetRepository,
            IRefTermRepository refTermRepository,
            ICategoryRepository categoryRepository,
            ILogger<DatabaseSeeder> logger
        )
        {
            _refSetRepository = refSetRepository;
            _refTermRepository = refTermRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously seeds all initial seed data into the database.
        /// </summary>
        public async Task SeedAsync()
        {
            _logger.LogInformation("=== Database seeding started ===");

            await SeedRefSetsAsync();
            await SeedRefTermsAsync();
            await SeedCategoriesAsync();

            _logger.LogInformation("=== Database seeding completed ===");
        }

        private async Task SeedRefSetsAsync()
        {
            const string resourceName =
                "ComplaintMaintenanceService.Infrastructure.Persistence.SeedData.ref_sets.csv";

            var records = ReadEmbeddedCsv<RefSetSeedDto>(resourceName);
            if (records is null)
                return;

            foreach (var r in records)
            {
                var id = r.Id;
                var code = r.Code.Trim();
                var description = r.Description.Trim();

                var existing = await _refSetRepository.GetByCodeAsync(code);
                if (existing is not null)
                {
                    if (existing.Description != description)
                    {
                        existing.Description = description;
                        await _refSetRepository.UpdateAsync(existing);
                        _logger.LogInformation("Updated RefSet '{Code}'.", code);
                    }
                    else
                    {
                        _logger.LogInformation("RefSet '{Code}' unchanged — skipping.", code);
                    }
                    continue;
                }

                await _refSetRepository.AddAsync(
                    new RefSet
                    {
                        Id = id,
                        Code = code,
                        Description = description,
                    }
                );

                _logger.LogInformation("Seeded RefSet '{Code}'.", code);
            }
        }

        private async Task SeedRefTermsAsync()
        {
            const string resourceName =
                "ComplaintMaintenanceService.Infrastructure.Persistence.SeedData.ref_terms.csv";

            var records = ReadEmbeddedCsv<RefTermSeedDto>(resourceName);
            if (records is null)
                return;

            foreach (var r in records)
            {
                var id = r.Id;
                var code = r.Code.Trim();
                var displayName = r.DisplayName.Trim();
                var refSetId = r.RefSetId;

                var existing = await _refTermRepository.GetByCodeAndSetIdAsync(code, refSetId);
                if (existing is not null)
                {
                    if (existing.DisplayName != displayName)
                    {
                        existing.DisplayName = displayName;
                        await _refTermRepository.UpdateAsync(existing);
                        _logger.LogInformation(
                            "Updated RefTerm '{Code}' under RefSet {RefSetId}.",
                            code,
                            refSetId
                        );
                    }
                    else
                    {
                        _logger.LogInformation(
                            "RefTerm '{Code}' under RefSet {RefSetId} unchanged — skipping.",
                            code,
                            refSetId
                        );
                    }
                    continue;
                }

                await _refTermRepository.AddAsync(
                    new RefTerm
                    {
                        Id = id,
                        Code = code,
                        DisplayName = displayName,
                        RefSetId = refSetId,
                    }
                );

                _logger.LogInformation(
                    "Seeded RefTerm '{Code}' under RefSet {RefSetId}.",
                    code,
                    refSetId
                );
            }
        }

        private async Task SeedCategoriesAsync()
        {
            const string resourceName =
                "ComplaintMaintenanceService.Infrastructure.Persistence.SeedData.category.csv";

            var records = ReadEmbeddedCsv<CategorySeedDto>(resourceName);
            if (records is null)
                return;

            foreach (var r in records)
            {
                var id = r.Id;
                var name = r.Name.Trim();
                var description = r.Description.Trim();
                var img = r.Img.Trim();

                var existing = await _categoryRepository.GetByNameAsync(name);
                if (existing is not null)
                {
                    var changed = false;

                    if (existing.Description != description)
                    {
                        existing.Description = description;
                        changed = true;
                    }

                    if (existing.Img != img)
                    {
                        existing.Img = img;
                        changed = true;
                    }

                    if (changed)
                    {
                        await _categoryRepository.UpdateAsync(existing);
                        _logger.LogInformation("Updated Category '{Name}'.", name);
                    }
                    else
                    {
                        _logger.LogInformation("Category '{Name}' unchanged — skipping.", name);
                    }
                    continue;
                }

                await _categoryRepository.AddAsync(
                    new Category
                    {
                        Id = id,
                        Name = name,
                        Description = description,
                        Img = img,
                    }
                );

                _logger.LogInformation("Seeded Category '{Name}'.", name);
            }
        }

        /// <summary>
        /// Reads an embedded CSV resource into a strongly-typed list using CsvHelper.
        /// Returns null (rather than throwing) if the resource isn't found, so one
        /// missing/renamed file doesn't abort the whole seeding run.
        /// </summary>
        private List<T>? ReadEmbeddedCsv<T>(string resourceName)
        {
            var assembly = typeof(DatabaseSeeder).Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                _logger.LogError(
                    "Embedded resource '{ResourceName}' not found. Confirm the file is included "
                        + "via <EmbeddedResource Include=\"Persistence\\SeedData\\*.csv\" /> in the .csproj "
                        + "and that the filename matches exactly (case-sensitive).",
                    resourceName
                );
                return null;
            }

            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            try
            {
                return csv.GetRecords<T>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to parse embedded CSV resource '{ResourceName}'.",
                    resourceName
                );
                return null;
            }
        }

        private class RefSetSeedDto
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        private class RefTermSeedDto
        {
            public Guid Id { get; set; }
            public string Code { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public Guid RefSetId { get; set; }
        }

        private class CategorySeedDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Img { get; set; } = string.Empty;
        }
    }
}