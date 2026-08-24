namespace ResidentVisitorService.Application.Constants;

public static class ResidentVisitorConstants
{
    public static class RefSetCodes
    {
        public const string VISIT_STATUS = "VISIT_STATUS";
        public const string VISITOR_TYPE = "VISITOR_TYPE";
        public const string PURPOSE_TYPE = "PURPOSE_TYPE";
    }

    public static class VisitStatus
    {
        public const string PENDING = "PENDING";
        public const string APPROVED = "APPROVED";
        public const string REJECTED = "REJECTED";
        public const string CHECKED_IN = "CHECKED_IN";
        public const string CHECKED_OUT = "CHECKED_OUT";
        public const string EXPIRED = "EXPIRED";
        public const string CANCELLED = "CANCELLED";
    }

    public static class Messages
    {
        public const string VisitorsFetched = "Visitors fetched successfully";
        public const string VisitorCreated = "Visitor created successfully";
        public const string VisitorUpdated = "Visitor updated successfully";
        public const string VisitorPhotoUploaded = "Visitor photo uploaded successfully";
        public const string VisitsFetched = "Visits fetched successfully";
        public const string VisitRegistered = "Visit registered successfully";
        public const string VisitUpdated = "Visit updated successfully";
        public const string VisitApproved = "Visit approved successfully";
        public const string VisitRejected = "Visit rejected successfully";
        public const string CheckedIn = "Visitor checked in successfully";
        public const string CheckedOut = "Visitor checked out successfully";
        public const string QrTokenGenerated = "QR token generated successfully";
        public const string QrTokenValidated = "QR token validated successfully";
        public const string VisitorTypesRetrieved = "Visitor types retrieved successfully";
        public const string PurposeTypesRetrieved = "Purpose types retrieved successfully";
    }

    public static class Errors
    {
        public const string VisitorNotFound = "Visitor with id '{0}' was not found.";
        public const string VisitorTypeNotFound = "Visitor type with id '{0}' was not found.";
        public const string PhoneNumberAlreadyExists =
            "A visitor with phone number '{0}' already exists.";
        public const string VisitorLookupFailed =
            "Visitor lookup by phone number failed unexpectedly.";
        public const string VisitNotFound = "Visit with id '{0}' was not found.";
        public const string VisitForQrNotFound = "Visit for QR token was not found.";
        public const string PurposeTypeNotFound = "Purpose type '{0}' was not found.";
        public const string VisitorOrDetailRequired =
            "Either VisitorId or Visitor details must be provided.";
        public const string EndDateBeforeStartDate = "End date must be on or after start date.";
        public const string DuplicateVisit =
            "An active visit already exists for this visitor with overlapping dates.";
        public const string PendingStatusNotConfigured =
            "Visit status 'PENDING' is not configured in reference data.";
        public const string ApprovedStatusNotConfigured =
            "Visit status 'APPROVED' is not configured in reference data.";
        public const string RejectedStatusNotConfigured =
            "Visit status 'REJECTED' is not configured in reference data.";
        public const string CheckedInStatusNotConfigured =
            "Visit status 'CHECKED_IN' is not configured in reference data.";
        public const string CheckedOutStatusNotConfigured =
            "Visit status 'CHECKED_OUT' is not configured in reference data.";
        public const string OnlyPendingCanBeApproved =
            "Only PENDING visits can be approved. Current status: {0}.";
        public const string OnlyPendingCanBeRejected =
            "Only PENDING visits can be rejected. Current status: {0}.";
        public const string OnlyPendingCanBeUpdated =
            "Only visits with PENDING status can be updated.";
        public const string OnlyApprovedCanBeCheckedIn =
            "Only APPROVED visits can be checked in. Current status: {0}.";
        public const string OnlyCheckedInCanBeCheckedOut =
            "Only CHECKED_IN visits can be checked out. Current status: {0}.";
        public const string VisitCannotBeCancelled = "Visit with status '{0}' cannot be cancelled.";
        public const string QrTokenNotFoundOrExpired = "QR token was not found or has expired.";
        public const string ActiveQrTokenAlreadyExists =
            "An active QR token already exists for this visit.";
        public const string QrOnlyForApprovedVisits =
            "QR token can only be generated for APPROVED visits. Current status: {0}.";
        public const string PhotoFileEmpty = "Uploaded file is empty.";
        public const string PhotoFileTooLarge =
            "File size exceeds the maximum allowed limit of 5 MB.";
        public const string PhotoInvalidExtension =
            "File type '{0}' is not allowed. Accepted types: {1}.";
        public const string JwtKeyMissing = "Jwt:Key is missing from configuration";
        public const string JwtIssuerMissing = "Jwt:Issuer is missing from configuration";
        public const string JwtAudienceMissing = "Jwt:Audience is missing from configuration";
        public const string FileStoragePathMissing =
            "FileStorage:VisitorPhotoPath is missing from configuration.";
        public const string NotificationUrlMissing =
            "NotificationService:GrpcUrl is missing from configuration.";
        public const string IdentityUrlMissing =
            "IdentityService:GrpcUrl is missing from configuration.";
        public const string BlockAndFlatRequired =
            "BlockNumber and FlatNumber are required when Security registers a walk-in visit.";
        public const string FlatNotFound = "No flat found for the given block and flat number.";
        public const string IdentityBaseUrlMissing =
            "IdentityService:BaseUrl is missing from configuration.";
        public const string QrTokenNotFound = "QR token was not found.";
        public const string AlreadyCheckedIn = "Visitor is already checked in.";
        public const string PhotoMismatch = "Captured photo does not match reference photo.";
        public const string NotCheckedIn = "Visitor is not checked in.";
        public const string AlreadyCheckedOut = "Visitor is already checked out.";
    }

    /// <summary>
    /// Sort defaults for paginated queries. Page/limit values live in
    /// Shared.SharedLibrary.Constants.PaginationConstants — use that instead of duplicating here.
    /// </summary>
    public static class Pagination
    {
        public const string DefaultSortBy = "createdAt";
        public const string DefaultSortOrder = "desc";
    }

    public static class Swagger
    {
        public const string ApiVersion = "v1";
        public const string ApiTitle = "Resident Visitor Service API";
        public const string BearerScheme = "Bearer";
        public const string ApiDescription =
            "API for managing residents, visitors, flats, and visits in the smart apartment community";
        public const string AuthHeaderName = "Authorization";
        public const string BearerFormat = "JWT";
        public const string BearerDescription = "Enter your JWT token. Example: Bearer {token}";
        public const string SwaggerEndpoint = "/swagger/v1/swagger.json";
        public const string SwaggerDisplayName = "Resident Visitor Service v1";
    }

    public static class CorsPolicy
    {
        public const string AllowAll = "AllowAll";
    }

    public static class PhotoUpload
    {
        public const long MaxFileSizeBytes = 5 * 1024 * 1024;
        public static readonly IReadOnlySet<string> AllowedExtensions = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase
        )
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
        };
    }

    public static class Database
    {
        public const string ConnectionStringName = "DefaultConnection";
        public const string SchemaName = "DB_TEAM_C_resident_visitor";
        public const string MigrationsHistoryTable = "__EFMigrationsHistory";
    }

    public static class TableNames
    {
        public const string RefSets = "ref_sets";
        public const string RefTerms = "ref_terms";
        public const string Visitors = "visitors";
        public const string Visits = "visits";
        public const string VisitQrTokens = "visit_qr_tokens";
    }

    public static class AuditFields
    {
        public const string CreatedAt = "CreatedAt";
        public const string CreatedBy = "CreatedBy";
        public const string UpdatedAt = "UpdatedAt";
        public const string UpdatedBy = "UpdatedBy";
        public const string IsActive = "IsActive";
    }

    public static class Seeder
    {
        public const string RefSetsCsvFile = "ref_sets.csv";
        public const string RefTermsCsvFile = "ref_terms.csv";
        public const string FlushSql = "SELECT 1";
        public const string EmbeddedResourceNotFound = "Embedded CSV resource '{0}' was not found.";
    }

    public static class FileStorage
    {
        public const string PhotoPathConfigKey = "FileStorage:VisitorPhotoPath";
    }

    public static class QrToken
    {
        public const string TokenFormat = "N";
    }

    public static class JwtClaims
    {
        public const string UserId = "userId";
        public const string Sub = "sub";
    }

    public static class Validation
    {
        public const string PageMustBePositive = "Page number must be greater than 0.";
        public const string LimitMustBePositive = "Limit must be greater than 0.";
        public const string LimitExceedsMax = "Limit cannot exceed 100.";
        public const string VisitorIdRequired = "Visitor ID is required.";
        public const string VisitIdRequired = "Visit ID is required.";
        public const string AtLeastOneFieldRequired =
            "At least one field must be provided to update.";
        public const string NameRequired = "Name is required.";
        public const string NameTooLong = "Name cannot exceed 100 characters.";
        public const string PhoneNumberRequired = "Phone number is required.";
        public const string InvalidPhoneNumber = "Phone number must be exactly 10 digits.";
        public const string InvalidEmail = "Email address is not valid.";
        public const string EmailTooLong = "Email cannot exceed 255 characters.";
        public const string EndDateBeforeStartDate = "End date must be on or after start date.";
        public const string PurposeTypeIdRequired = "Purpose type is required.";
        public const string StartDateRequired = "Start date is required.";
        public const string EndDateRequired = "End date is required.";
        public const string VisitorTypeIdRequired = "Visitor type is required.";
        public const string RejectionReasonRequired = "Rejection reason is required.";
        public const string RejectionReasonTooLong =
            "Rejection reason cannot exceed 500 characters.";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Resident = "Resident";
        public const string Security = "Security";
    }

    /// <summary>
    /// Notification type codes — must match the <c>notification_type</c> values
    /// seeded in <c>DB_TEAM_C_notification.notification_templates</c>.
    /// </summary>
    public static class NotificationTypes
    {
        public const string VISITOR_REGISTERED = "VISITOR_REGISTERED";
        public const string VISITOR_AT_GATE = "VISITOR_AT_GATE";

        public const string VISITOR_APPROVED = "VISITOR_APPROVED";
        public const string VISITOR_REJECTED = "VISITOR_REJECTED";
        public const string VISITOR_CHECKED_IN = "VISITOR_CHECKED_IN";
        public const string VISITOR_CHECKED_OUT = "VISITOR_CHECKED_OUT";
    }

    /// <summary>Notification title strings sent with each push notification.</summary>
    public static class NotificationTitles
    {
        public const string VISITOR_REGISTERED = "New Visitor Registration";
        public const string VISITOR_AT_GATE = "Visitor Awaiting Approval";
        public const string VISITOR_APPROVED = "Visitor Approved";
        public const string VISITOR_REJECTED = "Visitor Request Rejected";
        public const string VISITOR_CHECKED_IN = "Visitor Checked In";
        public const string VISITOR_CHECKED_OUT = "Visitor Checked Out";
    }

    /// <summary>
    /// Notification message templates. Use <c>string.Format</c> to substitute {0} = visitor name.
    /// </summary>
    public static class NotificationMessages
    {
        public const string VISITOR_REGISTERED = "Your visitor {0} has been registered.";
        public const string VISITOR_AT_GATE =
            "Your visitor {0} is at the gate and awaiting your approval.";
        public const string VISITOR_APPROVED =
            "Your visitor {0} has been approved. A QR pass has been generated.";
        public const string VISITOR_REJECTED = "Your visitor {0} visit request has been rejected.";
        public const string VISITOR_CHECKED_IN = "{0} has checked in at the gate.";
        public const string VISITOR_CHECKED_OUT = "{0} has checked out.";
    }

    public static class NotificationService
    {
        public const string UrlConfigKey = "NotificationService:GrpcUrl";
    }

    public static class IdentityService
    {
        public const string GrpcUrlConfigKey = "IdentityService:GrpcUrl";
    }
}
