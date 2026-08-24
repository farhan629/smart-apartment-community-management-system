namespace ComplaintMaintenanceService.Application.Common.Constants;

/// <summary>
/// Constants for gRPC log messages and response messages used across CmsGrpcService.
/// </summary>
public static class GrpcMessages
{
    public const string GetCategoryCalled = "gRPC GetCategory called for CategoryId: {CategoryId}";
    public const string InvalidCategoryIdFormat = "Invalid CategoryId format: {CategoryId}";
    public const string CategoryNotFound = "Category not found for Id: {CategoryId}";
    public const string CreateStaffCalled = "gRPC CreateStaff called for UserId: {UserId}";
    public const string InvalidUserOrCategoryId = "Invalid UserId or CategoryId format";
    public const string StaffAlreadyExists = "Staff already exists for UserId: {UserId}";
    public const string StaffCreatedSuccess = "Staff created successfully with Id: {StaffId}";
    public const string StaffAlreadyExistsMessage = "Staff already exists for this user";
    public const string StaffCreatedMessage = "Staff created successfully";
    public const string InvalidIdsMessage = "Invalid UserId or CategoryId";
}



/// <summary>
/// Configuration key constants for gRPC client settings, shared across all CMS gRPC clients.
/// </summary>
public static class GrpcConfigKeys
{
    public const string IdentityServiceUrl = "GrpcSettings:IdentityServiceUrl";
    public const string NotificationServiceUrl = "GrpcSettings:NotificationServiceUrl";
    public const string FlatLookupServiceUrl = "GrpcSettings:FlatLookupServiceUrl";
    public const string StaffServiceUrl = "GrpcSettings:StaffServiceUrl";

    public const string IdentityServiceUrlMissing =
        "GrpcSettings:IdentityServiceUrl is not configured.";
    public const string NotificationServiceUrlMissing =
        "GrpcSettings:NotificationServiceUrl is not configured.";
    public const string FlatLookupServiceUrlMissing =
        "GrpcSettings:FlatLookupServiceUrl is not configured.";
    public const string StaffServiceUrlMissing = "GrpcSettings:StaffServiceUrl is not configured.";
}
