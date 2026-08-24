namespace IdentityService.Application.Interfaces.Services;

/// <summary>
/// Defines the contract for communicating with CMS via gRPC.
/// Used by Identity to validate categories and create staff profiles in CMS.
/// </summary>
public interface ICmsGrpcClient
{
    /// <summary>
    /// Retrieves category details from CMS by category ID.
    /// Returns null if category is not found.
    /// </summary>
    Task<CmsGrpcCategoryResponse?> GetCategoryAsync(Guid categoryId, CancellationToken ct = default);

    /// <summary>
    /// Creates a staff profile in CMS after user creation in Identity.
    /// Returns the created staff ID.
    /// </summary>
    Task<CmsGrpcCreateStaffResponse> CreateStaffAsync(CmsGrpcCreateStaffRequest request, CancellationToken ct = default);
}

/// <summary>
/// Represents category data returned from CMS via gRPC.
/// </summary>
public record CmsGrpcCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    string Img
);

/// <summary>
/// Represents the request to create a staff profile in CMS via gRPC.
/// </summary>
public record CmsGrpcCreateStaffRequest(
    Guid UserId,
    Guid CategoryId,
    string Description,
    string Details
);

/// <summary>
/// Represents the response from CMS after staff creation via gRPC.
/// </summary>
public record CmsGrpcCreateStaffResponse(
    Guid StaffId,
    bool Success,
    string Message
);
