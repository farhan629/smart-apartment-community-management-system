using System;

namespace Shared.SharedLibrary.Constants
{
    /// <summary>
    /// Contains exception message constants.
    /// </summary>
    public static class ExceptionMessages
    {
        public const string AlreadyFlatOccupied = "Already Flat Occupied";
        public const string EmailNotEntered = "Email Not Entered";

        /// <summary>Email is not registered.</summary>
        public const string EmailNotRegistered = "Email is not registered.";

        /// <summary>Invalid password.</summary>
        public const string InvalidPassword = "Invalid password.";

        /// <summary>Account is deactivated.</summary>
        public const string AccountDeactivated = "Account is deactivated.";

        /// <summary>Invalid refresh token.</summary>
        public const string InvalidRefreshToken = "Invalid refresh token.";

        /// <summary>Refresh token expired.</summary>
        public const string RefreshTokenExpired = "Refresh token expired.";

        /// <summary>Email already exists.</summary>
        public const string EmailAlreadyExists = "Email already exists.";

        /// <summary>Invalid role.</summary>
        public const string InvalidRole = "Invalid role.";

        /// <summary>Staff category is required.</summary>
        public const string StaffCategoryRequired = "Staff category is required.";

        /// <summary>Flat is required.</summary>
        public const string FlatRequired = "Flat is required.";

        /// <summary>A pending occupancy request already exists for this flat.</summary>
        public const string FlatOccupancyPendingExists =
            "A pending occupancy request already exists for this flat.";

        /// <summary>Your flat occupancy request is pending admin approval.</summary>
        public const string OccupancyNotApproved =
            "Your flat occupancy request is pending admin approval.";

        /// <summary>Not Found.</summary>
        public const string NotFound = "Not Found.";

        /// <summary>Approval record.</summary>
        public const string ApprovalRecordEntityName = "Approval record";

        /// <summary>Role '{0}' already exists in category '{1}'.</summary>
        public const string RoleAlreadyExistsInCategory =
            "Role '{0}' already exists in category '{1}'.";

        /// <summary>{0} not found.</summary>
        public const string EntityNotFound = "{0} not found.";

        /// <summary>JWT Key is not configured.</summary>
        public const string JwtKeyNotConfigured = "JWT Key is not configured.";

        /// <summary>Jwt:ExpiryMinutes is missing or invalid in configuration.</summary>
        public const string JwtExpiryMinutesNotConfigured =
            "Jwt:ExpiryMinutes is missing or invalid in configuration.";

        /// <summary>Password cannot be empty.</summary>
        public const string PasswordCannotBeEmpty = "Password cannot be empty.";

        /// <summary>Phone number is not registered.</summary>
        public const string PhoneNotRegistered = "Phone number is not registered.";

        /// <summary>OTP has expired. Please request a new one.</summary>
        public const string OtpExpired = "OTP has expired. Please request a new one.";

        /// <summary>Invalid OTP provided.</summary>
        public const string InvalidOtp = "Invalid OTP provided.";

        /// <summary>Token has expired.</summary>
        public const string TokenExpired = "Token has expired.";

        /// <summary>Credentials not found.</summary>
        public const string CredentialsNotFound = "Credentials not found.";

        /// <summary>Current password is incorrect.</summary>
        public const string CurrentPasswordIncorrect = "Current password is incorrect.";

        /// <summary>New password cannot be the same as the current password.</summary>
        public const string PasswordReused =
            "New password cannot be the same as the current password.";

        /// <summary>Too many OTP attempts. Please try again later.</summary>
        public const string TooManyOtpAttempts = "Too many OTP attempts. Please try again later.";
    }

    /// <summary>
    /// Contains reference set IDs.
    /// </summary>
    public static class RefSetIds
    {
        /// <summary>Management set ID.</summary>
        public static readonly Guid ManagementSetId = new Guid(
            "11111111-1111-1111-1111-111111111111"
        );

        /// <summary>Occupant set ID.</summary>
        public static readonly Guid OccupantSetId = new Guid(
            "22222222-2222-2222-2222-222222222222"
        );
    }

    /// <summary>
    /// Contains configuration key constants.
    /// </summary>
    public static class ConfigKeys
    {
        /// <summary>Default connection string key.</summary>
        public const string DefaultConnection = "DefaultConnection";

        /// <summary>JWT Key configuration key.</summary>
        public const string JwtKey = "Jwt:Key";

        /// <summary>JWT Issuer configuration key.</summary>
        public const string JwtIssuer = "Jwt:Issuer";

        /// <summary>JWT Audience configuration key.</summary>
        public const string JwtAudience = "Jwt:Audience";

        /// <summary>JWT Expiry top-level fallback key.</summary>
        public const string JwtExpiry = "Jwt:Expiry";

        /// <summary>JWT section name.</summary>
        public const string JwtSectionName = "Jwt";

        /// <summary>JWT Key sub-key.</summary>
        public const string JwtKeySubKey = "Key";

        /// <summary>JWT Issuer sub-key.</summary>
        public const string JwtIssuerSubKey = "Issuer";

        /// <summary>JWT Audience sub-key.</summary>
        public const string JwtAudienceSubKey = "Audience";

        /// <summary>JWT ExpiryMinutes sub-key.</summary>
        public const string JwtExpiryMinutesSubKey = "ExpiryMinutes";

        /// <summary>JWT RefreshExpiryDays sub-key.</summary>
        public const string JwtRefreshExpiryDaysSubKey = "RefreshExpiryDays";

        /// <summary>gRPC Complaint Service URL configuration key.</summary>
        public const string GrpcComplaintServiceUrl = "GrpcSettings:ComplaintServiceUrl";
    }

    /// <summary>
    /// Contains configuration error messages.
    /// </summary>
    public static class ConfigErrorMessages
    {
        /// <summary>DefaultConnection is missing from configuration.</summary>
        public const string DefaultConnectionMissing =
            "DefaultConnection is missing from configuration";

        /// <summary>Jwt:Key is missing from configuration.</summary>
        public const string JwtKeyMissing = "Jwt:Key is missing from configuration";

        /// <summary>Jwt:Issuer is missing from configuration.</summary>
        public const string JwtIssuerMissing = "Jwt:Issuer is missing from configuration";

        /// <summary>Jwt:Audience is missing from configuration.</summary>
        public const string JwtAudienceMissing = "Jwt:Audience is missing from configuration";
    }

    /// <summary>
    /// Contains database constants.
    /// </summary>
    public static class DbConstants
    {
        /// <summary>Migrations history table name.</summary>
        public const string MigrationsHistoryTable = "__EFMigrationsHistory";

        /// <summary>Migrations history schema.</summary>
        public const string MigrationsHistorySchema = "DB_TEAM_C_identity";

        /// <summary>Amenity Booking Service database schema.</summary>
        public const string AmenitySchema = "DB_TEAM_C_amenity";
    }

    /// <summary>
    /// Contains logging constants.
    /// </summary>
    public static class LoggingConstants
    {
        /// <summary>Log file path template.</summary>
        public const string LogFilePathTemplate = "logs/identity-service-log-.txt";

        /// <summary>Amenity Booking Service log file path template.</summary>
        public const string AmenityLogFilePathTemplate = "logs/amenity-booking-log-.txt";
    }

    /// <summary>
    /// Contains CORS policy constants.
    /// </summary>
    public static class CorsPolicies
    {
        /// <summary>Allow all CORS policy.</summary>
        public const string AllowAll = "AllowAll";
    }

    /// <summary>
    /// Stores Swagger and OpenAPI related configuration values.
    /// Stores Swagger and OpenAPI related configuration values.
    /// Stores Swagger and OpenAPI related configuration values.
    /// </summary>
    public static class SwaggerConstants
    {
        /// <summary>Swagger document name.</summary>
        public const string DocName = "v1";

        /// <summary>Identity Service Swagger title.</summary>
        /// <summary>Identity Service Swagger title.</summary>
        /// <summary>Identity Service Swagger title.</summary>
        public const string Title = "Identity Service API";

        /// <summary>Swagger version.</summary>
        public const string Version = "v1";

        /// <summary>Identity Service Swagger description.</summary>
        public const string Description =
            "API for user authentication, authorization, and identity management in the smart apartment community";

        /// <summary>Security scheme name.</summary>
        public const string SecuritySchemeName = "Bearer";

        /// <summary>Security scheme.</summary>
        public const string SecurityScheme = "Bearer";

        /// <summary>Bearer format.</summary>
        public const string BearerFormat = "JWT";

        /// <summary>Auth header name.</summary>
        public const string AuthHeaderName = "Authorization";

        /// <summary>Auth description.</summary>
        public const string AuthDescription = "Enter your JWT token. Example: Bearer {token}";

        /// <summary>Swagger JSON endpoint.</summary>
        public const string SwaggerJsonEndpoint = "/swagger/v1/swagger.json";

        /// <summary>Identity Service Swagger UI title.</summary>
        /// <summary>Identity Service Swagger UI title.</summary>
        /// <summary>Identity Service Swagger UI title.</summary>
        public const string SwaggerUiTitle = "Identity Service v1";

        /// <summary>Amenity Booking Service Swagger title.</summary>
        public const string AmenityTitle = "Amenity Booking Service API";

        /// <summary>Amenity Booking Service Swagger version.</summary>
        public const string AmenityVersion = "v1";

        /// <summary>Amenity Booking Service Swagger description.</summary>
        public const string AmenityDescription =
            "API for managing amenity bookings in the smart apartment community";

        /// <summary>Amenity Booking Service Swagger UI title.</summary>
        public const string AmenitySwaggerUiTitle = "Amenity Booking Service v1";
    }

    /// <summary>
    /// Contains OpenAPI schema constants.
    /// </summary>
    public static class OpenApiSchemaConstants
    {
        /// <summary>TimeSpan schema type.</summary>
        public const string TimeSpanType = "string";

        /// <summary>TimeSpan schema format.</summary>
        public const string TimeSpanFormat = "time-span";

        /// <summary>TimeSpan schema example.</summary>
        public const string TimeSpanExample = "06:00:00";
    }

    /// <summary>
    /// Contains application startup log messages.
    /// </summary>
    public static class StartupLogMessages
    {
        /// <summary>Amenity Booking Service startup log.</summary>
        public const string AmenityServiceStarting = "Amenity Booking Service starting up...";
    }

    /// <summary>
    /// Contains approval response messages.
    /// </summary>
    public static class ApprovalMessages
    {
        /// <summary>Approved successfully message.</summary>
        public const string ApprovedSuccessfully = "User has been approved successfully";

        /// <summary>Rejected successfully message.</summary>
        public const string RejectedSuccessfully = "User registration has been rejected";
    }

    /// <summary>
    /// Contains configuration default values.
    /// </summary>
    public static class ConfigDefaults
    {
        /// <summary>Refresh token expiry days.</summary>
        public const string RefreshTokenExpiryDays = "7";

        /// <summary>Complaint service URL.</summary>
        public const string ComplaintServiceUrl = "http://localhost:5223";

        /// <summary>Refresh token expiry days fallback value.</summary>
        public const int RefreshTokenExpiryDaysFallback = 2;
    }

    /// <summary>
    /// Custom JWT claim names used when generating and validating tokens.
    /// Custom JWT claim names used when generating and validating tokens.
    /// Custom JWT claim names used when generating and validating tokens.
    /// </summary>
    public static class JwtClaimTypes
    {
        /// <summary>Role ID claim.</summary>
        public const string RoleId = "roleId";

        /// <summary>Token type claim.</summary>
        public const string TokenType = "token_type";
    }

    /// <summary>
    /// Contains JWT token type constants.
    /// </summary>
    public static class JwtTokenTypes
    {
        /// <summary>Refresh token type.</summary>
        public const string Refresh = "refresh";
    }

    /// <summary>
    /// Contains gRPC staff messages.
    /// </summary>
    public static class GrpcStaffMessages
    {
        /// <summary>Create staff description.</summary>
        public const string CreateStaffDescription = "Propagated via IdentityService registration.";

        /// <summary>Create staff details.</summary>
        public const string CreateStaffDetails = "Staff member added.";
    }

    /// <summary>
    /// Contains password hashing constants.
    /// </summary>
    public static class PasswordConstants
    {
        /// <summary>BCrypt work factor.</summary>
        public const int WorkFactor = 12;
    }

    /// <summary>
    /// Contains validation error messages and numeric constraints.
    /// </summary>
    public static class ValidationConstants
    {
        #region Password Validation Constants
        /// <summary>Current password is required.</summary>
        public const string CurrentPasswordRequired = "Current password is required.";

        /// <summary>New password is required.</summary>
        public const string NewPasswordRequired = "New password is required.";

        /// <summary>Password is required.</summary>
        public const string PasswordRequired = "Password is required.";

        /// <summary>Confirm password is required.</summary>
        public const string ConfirmPasswordRequired = "Confirm password is required.";

        /// <summary>Passwords do not match.</summary>
        public const string PasswordsDoNotMatch = "Passwords do not match.";

        /// <summary>Password must be at least 8 characters.</summary>
        public const string PasswordMinLengthMessage = "Password must be at least 8 characters.";

        /// <summary>Password must contain at least one uppercase letter.</summary>
        public const string PasswordUppercaseMessage =
            "Password must contain at least one uppercase letter.";

        /// <summary>Password must contain at least one number.</summary>
        public const string PasswordDigitMessage = "Password must contain at least one number.";

        /// <summary>Password must contain at least one special character.</summary>
        public const string PasswordSpecialCharMessage =
            "Password must contain at least one special character.";

        /// <summary>Password minimum length constraint.</summary>
        public const int PasswordMinLength = 8;
        #endregion

        #region Email Validation Constants
        /// <summary>Email is required.</summary>
        public const string EmailRequired = "Email is required.";

        /// <summary>Email cannot exceed 100 characters.</summary>
        public const string EmailMaxLengthMessage = "Email cannot exceed 100 characters.";

        /// <summary>Email must contain only lowercase letters, digits, and allowed special characters.</summary>
        public const string EmailAllowedCharsMessage =
            "Email must contain only lowercase letters, digits, and allowed special characters.";

        /// <summary>Email must contain exactly one '@' symbol.</summary>
        public const string EmailOneAtMessage = "Email must contain exactly one '@' symbol.";

        /// <summary>Email cannot contain spaces.</summary>
        public const string EmailNoSpacesMessage = "Email cannot contain spaces.";

        /// <summary>Email cannot start or end with '@' or '.'.</summary>
        public const string EmailNoStartEndSpecialMessage =
            "Email cannot start or end with '@' or '.'.";

        /// <summary>Email must have a username before '@'.</summary>
        public const string EmailUsernameRequiredMessage = "Email must have a username before '@'.";

        /// <summary>Email must have a valid domain with a dot.</summary>
        public const string EmailDomainDotRequiredMessage =
            "Email must have a valid domain with a dot (e.g., .com, .org).";

        /// <summary>Email must end with a valid domain extension.</summary>
        public const string EmailDomainExtensionMessage =
            "Email must end with a valid domain extension (e.g., .com, .org, .net).";

        /// <summary>Invalid email format.</summary>
        public const string EmailInvalidFormatMessage = "Invalid email format.";

        /// <summary>Email maximum length constraint.</summary>
        public const int EmailMaxLength = 100;

        /// <summary>Email regex pattern.</summary>
        public const string EmailRegexPattern = @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$";
        #endregion

        #region Phone Validation Constants
        /// <summary>Phone number is required.</summary>
        public const string PhoneRequired = "Phone number is required.";

        /// <summary>Phone number must be at least 10 digits if provided.</summary>
        public const string PhoneMinLengthMessage =
            "Phone number must be at least 10 digits if provided.";

        /// <summary>Phone number cannot exceed 15 characters.</summary>
        public const string PhoneMaxLengthMessage = "Phone number cannot exceed 15 characters.";

        /// <summary>Phone number contains invalid characters.</summary>
        public const string PhoneInvalidCharsMessage = "Phone number contains invalid characters.";

        /// <summary>Phone number minimum length constraint.</summary>
        public const int PhoneMinLength = 10;

        /// <summary>Phone number maximum length constraint.</summary>
        public const int PhoneMaxLength = 15;
        #endregion

        #region OTP Validation Constants
        /// <summary>OTP is required.</summary>
        public const string OtpRequired = "OTP is required.";

        /// <summary>OTP must be exactly 6 digits.</summary>
        public const string OtpLengthMessage = "OTP must be exactly 6 digits.";

        /// <summary>OTP must contain only digits.</summary>
        public const string OtpDigitsOnlyMessage = "OTP must contain only digits.";

        /// <summary>OTP length constraint.</summary>
        public const int OtpLength = 6;

        /// <summary>OTP regex pattern.</summary>
        public const string OtpRegexPattern = @"^\d{6}$";
        #endregion

        #region Reset Password Validation Constants
        /// <summary>Reset token is required.</summary>
        public const string ResetTokenRequired = "Reset token is required.";

        /// <summary>Username must be at least 3 characters if provided.</summary>
        public const string UsernameMinLengthMessage =
            "Username must be at least 3 characters if provided.";

        /// <summary>Username cannot exceed 50 characters.</summary>
        public const string UsernameMaxLengthMessage = "Username cannot exceed 50 characters.";

        /// <summary>Username can only contain letters, digits, underscores, and hyphens.</summary>
        public const string UsernameAllowedCharsMessage =
            "Username can only contain letters, digits, underscores, and hyphens.";

        /// <summary>Photo URL must be a valid absolute URL if provided.</summary>
        public const string PhotoUrlAbsoluteMessage =
            "Photo URL must be a valid absolute URL if provided.";

        /// <summary>Photo URL must start with http:// or https://.</summary>
        public const string PhotoUrlSchemeMessage =
            "Photo URL must start with http:// or https://.";

        /// <summary>Photo URL cannot exceed 500 characters.</summary>
        public const string PhotoUrlMaxLengthMessage = "Photo URL cannot exceed 500 characters.";

        /// <summary>Username minimum length constraint.</summary>
        public const int UsernameMinLength = 3;

        /// <summary>Username maximum length constraint.</summary>
        public const int UsernameMaxLength = 50;

        /// <summary>Photo URL maximum length constraint.</summary>
        public const int PhotoUrlMaxLength = 500;

        /// <summary>HTTPS URL prefix constraint.</summary>
        public const string PhotoUrlHttpsPrefix = "https://";

        /// <summary>HTTP URL prefix constraint.</summary>
        public const string PhotoUrlHttpPrefix = "http://";
        #endregion
    }

    public static class SeedDataResourceNames
    {
        public const string RefSets =
            "IdentityService.Infrastructure.Persistence.SeedData.ref_sets.csv";

        public const string RefTerms =
            "IdentityService.Infrastructure.Persistence.SeedData.ref_terms.csv";

        public const string Flats = "IdentityService.Infrastructure.Persistence.SeedData.flats.csv";

        public const string Users = "IdentityService.Infrastructure.Persistence.SeedData.users.csv";

        public const string RolePolicies =
            "IdentityService.Infrastructure.Persistence.SeedData.role_policies.csv";
    }

    public static class RoleIds
    {
        public static readonly Guid Resident = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public static readonly Guid Tenant = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        public static readonly Guid Staff = new("dddddddd-dddd-dddd-dddd-dddddddddddd");
        public static readonly Guid Admin = new("cccccccc-cccc-cccc-cccc-cccccccccccc");
    }
}
