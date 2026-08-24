namespace IdentityService.Application.Common.Constants;

/// <summary>
/// Constants for staff-related messages, log strings, and validation messages used across staff features.
/// </summary>
public static class StaffConstants
{
    // gRPC log messages
    public const string CategoryNotFound = "Category not found in CMS";
    public const string UserAlreadyExists = "A user with this email already exists";
    public const string ValidatingCategory = "Validating category {CategoryId} via CMS gRPC";
    public const string UserCreated = "User created with Id: {UserId}";
    public const string StaffCreatedInCms = "Staff created in CMS with Id: {StaffId}";
    public const string CmsUrlMissing = "GrpcSettings:CmsServiceUrl is missing from configuration";
    public const string CallingGetCategory = "Calling CMS gRPC GetCategory for CategoryId: {CategoryId}";
    public const string CategoryNotFoundInCms = "Category not found in CMS for Id: {CategoryId}";
    public const string CallingCreateStaff = "Calling CMS gRPC CreateStaff for UserId: {UserId}";

    // Validation messages
    public const string NameRequired = "Name is required";
    public const string NameMaxLength = "Name must not exceed 100 characters";
    public const string EmailRequired = "Email is required";
    public const string EmailInvalid = "A valid email address is required";
    public const string PhoneRequired = "Phone is required";
    public const string PhoneInvalid = "A valid phone number is required";
    public const string PasswordRequired = "Password is required";
    public const string PasswordMinLength = "Password must be at least 8 characters";
    public const string PasswordComplexity = "Password must contain uppercase, lowercase, number and special character";
    public const string CategoryIdRequired = "CategoryId is required";
    public const string DescriptionMaxLength = "Description must not exceed 500 characters";
    public const string DetailsMaxLength = "Details must not exceed 500 characters";
}
