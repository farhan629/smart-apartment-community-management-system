namespace NotificationService.Application.Constants;

public static class NotificationConstants
{
    public const string SCHEMA_NAME = "DB_TEAM_C_notification";
    public const string DEFAULT_NOTIFICATION_TYPE = "general";

    public static class NotificationStatus
    {
        public const string SENT = "sent";
        public const string FAILED = "failed";
        public const string PENDING = "pending";
    }

    public static class EmailLogStatus
    {
        public const string SENT = "sent";
        public const string FAILED = "failed";
    }

    public static class TemplatePlaceholders
    {
        public const string RECIPIENT_NAME = "{{recipientName}}";
        public const string TITLE_KEY = "title";
        public const string MESSAGE_KEY = "message";
    }

    public static class RefSetCodes
    {
        public const string NOTIFICATION_TYPE = "NOTIFICATION_TYPE";
    }

    public static class CorsPolicy
    {
        public const string ALLOW_ALL = "AllowAll";
        public const string DEFAULT_ORIGIN = "http://localhost:4200";
    }

    public static class Routes
    {
        public const string MARK_ALL_READ = "mark-all-read";
        public const string DELETE_ALL = "delete-all";
    }

    public static class Swagger
    {
        public const string API_VERSION = "v1";
        public const string API_TITLE = "Notification Service API";
        public const string API_DESCRIPTION =
            "API for managing notifications and emails in the smart apartment community";
        public const string SWAGGER_UI_TITLE = "Notification Service v1";
        public const string BEARER_SCHEME = "Bearer";
        public const string BEARER_FORMAT = "JWT";
        public const string AUTH_HEADER_NAME = "Authorization";
        public const string AUTH_DESCRIPTION = "Enter your JWT token. Example: Bearer {token}";
    }

    public static class SignalR
    {
        public const string HUB_PATH = "/notification-hub";
        public const string ACCESS_TOKEN_QUERY_PARAM = "access_token";
        public const string RECEIVE_NOTIFICATION = "ReceiveNotification";
    }

    public static class JwtClaims
    {
        public const string USER_ID = "userId";
        public const string SUB = "sub";
    }

    public static class ConfigKeys
    {
        public const string JWT_KEY = "Jwt:Key";
        public const string JWT_ISSUER = "Jwt:Issuer";
        public const string JWT_AUDIENCE = "Jwt:Audience";
        public const string CORS_ORIGIN = "Cors:Origin";
    }

    public static class EmailSettings
    {
        public const string SECTION_NAME = "EmailSettings";
        public const string DEFAULT_PORT = "587";
        public const string DEFAULT_SENDER_NAME = "Smart Apartment System";
    }

    public static class EntityNames
    {
        public const string NOTIFICATION = "Notification";
    }

    public static class Errors
    {
        public const string TITLE_AND_MESSAGE_REQUIRED = "Title and message are required.";
        public const string NOTIFICATION_NOT_FOUND = "Notification not found.";
        public const string EMAIL_SEND_FAILED = "Failed to send email to";
        public const string EMAIL_SETTINGS_HOST_MISSING = "EmailSettings:Host not configured.";
        public const string EMAIL_SETTINGS_SENDER_MISSING =
            "EmailSettings:SenderEmail not configured.";
        public const string EMAIL_SETTINGS_PASSWORD_MISSING =
            "EmailSettings:Password not configured.";
        public const string JWT_KEY_MISSING = "Jwt:Key is missing from configuration";
        public const string JWT_ISSUER_MISSING = "Jwt:Issuer is missing from configuration";
        public const string JWT_AUDIENCE_MISSING = "Jwt:Audience is missing from configuration";
    }

    public static class ValidationMessages
    {
        public const string USER_ID_REQUIRED = "UserId is required.";
        public const string TEMPLATE_ID_REQUIRED = "TemplateId is required.";
        public const string TITLE_REQUIRED = "Title is required.";
        public const string TITLE_MAX_LENGTH = "Title must not exceed 200 characters.";
        public const string MESSAGE_REQUIRED = "Message is required.";
        public const string MESSAGE_MAX_LENGTH = "Message must not exceed 1000 characters.";
        public const string RECIPIENT_EMAIL_INVALID =
            "RecipientEmail must be a valid email address.";
        public const string RECIPIENT_EMAIL_REQUIRED = "RecipientEmail is required.";
        public const string RECIPIENT_NAME_REQUIRED = "RecipientName is required.";
        public const string NOTIFICATION_TYPE_REQUIRED = "NotificationType is required.";
        public const string PAGE_MUST_BE_POSITIVE = "Page must be greater than 0.";
        public const string LIMIT_MUST_BE_POSITIVE = "Limit must be greater than 0.";
        public const string LIMIT_MAX = "Limit must not exceed 100.";
    }

    public static class Logging
    {
        public const string LOG_FILE_PATH = "logs/notification-service-log-.txt";
        public const string STARTUP_MESSAGE = "Notification Service starting up...";
    }

    public static class SeedData
    {
        public const string NOTIFICATION_TYPE_DESCRIPTION = "Notification Type";

        public const string NOTIFICATION_TYPES_RESOURCE_NAME =
            "NotificationService.Infrastructure.SeedData.notification_types.csv";

        public const string EMAIL_TEMPLATES_RESOURCE_NAME =
            "NotificationService.Infrastructure.SeedData.email_templates.csv";

        public const string NOTIFICATION_TEMPLATES_RESOURCE_NAME =
            "NotificationService.Infrastructure.SeedData.notification_templates.csv";
    }
}
