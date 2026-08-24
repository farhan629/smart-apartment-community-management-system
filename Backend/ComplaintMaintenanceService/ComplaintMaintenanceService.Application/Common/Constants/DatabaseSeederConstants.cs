namespace ComplaintMaintenanceService.Application.Common.Constants
{
    /// <summary>
    /// Constants for embedded CSV resource names used by <see cref="Persistence.Seeders.DatabaseSeeder"/>.
    /// </summary>
    public static class DatabaseSeederConstants
    {
        private const string SeedDataNamespace =
            "ComplaintMaintenanceService.Infrastructure.Persistence.SeedData";

        public const string RefSetsCsvResource = $"{SeedDataNamespace}.ref_sets.csv";
        public const string RefTermsCsvResource = $"{SeedDataNamespace}.ref_terms.csv";
        public const string CategoryCsvResource = $"{SeedDataNamespace}.category.csv";
        public const string EmbeddedResourceNotFound = "Embedded resource '{0}' not found.";
    }
}
