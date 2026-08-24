namespace ComplaintMaintenanceService.API.Constants;

public static class ApiStartupConstants
{
    public static class Logging
    {
        public const string LogFilePathTemplate = "logs/complaint-maintenance-log-.txt";
    }

    public static class Database
    {
        public const string DefaultConnectionKey = "DefaultConnection";
        public const string DefaultConnectionMissing =
            "DefaultConnection is missing from configuration";
        public const string MigrationsHistoryTable = "__EFMigrationsHistory";
        public const string Schema = "DB_TEAM_C_complaint";
    }

    public static class Cors
    {
        public const string AllowAllPolicy = "AllowAll";
    }

    public static class Swagger
    {
        public const string DocName = "v1";
        public const string ApiTitle = "Complaint Maintenance Service API";
        public const string ApiDescription =
            "API for managing complaint and maintenance requests in the smart apartment community";
        public const string SwaggerUiTitle = "Complaint Maintenance Service v1";
    }

    public static class Jwt
    {
        public const string KeyConfigPath = "Jwt:Key";
        public const string IssuerConfigPath = "Jwt:Issuer";
        public const string AudienceConfigPath = "Jwt:Audience";
        public const string KeyMissing = "Jwt:Key is missing from configuration";
        public const string IssuerMissing = "Jwt:Issuer is missing from configuration";
        public const string AudienceMissing = "Jwt:Audience is missing from configuration";
    }

    public static class Security
    {
        public const string BearerScheme = "Bearer";
        public const string AuthorizationHeaderName = "Authorization";
        public const string JwtBearerFormat = "JWT";
        public const string BearerDescription = "Enter your JWT token. Example: Bearer {token}";
    }

    public static class IdentityService
    {
        public const string BaseUrlConfigPath = "IdentityService:BaseUrl";
    }
}
