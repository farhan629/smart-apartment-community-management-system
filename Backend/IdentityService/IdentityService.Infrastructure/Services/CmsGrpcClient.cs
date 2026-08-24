using ComplaintMaintenanceService.Infrastructure.Protos;
using Grpc.Net.Client;
using IdentityService.Application.Common.Constants;
using IdentityService.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Services;

/// <summary>
/// gRPC client implementation for communicating with CMS service.
/// Handles category retrieval and staff creation via gRPC.
/// </summary>
public class CmsGrpcClient : ICmsGrpcClient
{
    private readonly CmsService.CmsServiceClient _client;
    private readonly ILogger<CmsGrpcClient> _logger;

    public CmsGrpcClient(IConfiguration configuration, ILogger<CmsGrpcClient> logger)
    {
        _logger = logger;
        var url =
            configuration["GrpcSettings:CmsServiceUrl"]
            ?? throw new InvalidOperationException(StaffConstants.CmsUrlMissing);
        var channel = GrpcChannel.ForAddress(url);
        _client = new CmsService.CmsServiceClient(channel);
    }

    /// <summary>
    /// Calls CMS gRPC to get category details by ID.
    /// Returns null if category not found.
    /// </summary>
    public async Task<CmsGrpcCategoryResponse?> GetCategoryAsync(
        Guid categoryId,
        CancellationToken ct = default
    )
    {
        _logger.LogInformation(StaffConstants.CallingGetCategory, categoryId);

        var response = await _client.GetCategoryAsync(
            new GetCategoryRequest { CategoryId = categoryId.ToString() },
            cancellationToken: ct
        );

        if (!response.Found)
        {
            _logger.LogWarning(StaffConstants.CategoryNotFoundInCms, categoryId);
            return null;
        }

        return new CmsGrpcCategoryResponse(
            Guid.Parse(response.Id),
            response.Name,
            response.Description,
            response.Img
        );
    }

    /// <summary>
    /// Calls CMS gRPC to create a staff profile after user creation in Identity.
    /// </summary>
    public async Task<CmsGrpcCreateStaffResponse> CreateStaffAsync(
        CmsGrpcCreateStaffRequest request,
        CancellationToken ct = default
    )
    {
        _logger.LogInformation(StaffConstants.CallingCreateStaff, request.UserId);

        var response = await _client.CreateStaffAsync(
            new CreateStaffRequest
            {
                UserId = request.UserId.ToString(),
                CategoryId = request.CategoryId.ToString(),
                Description = request.Description,
                Details = request.Details,
            },
            cancellationToken: ct
        );

        return new CmsGrpcCreateStaffResponse(
            Guid.Parse(response.StaffId),
            response.Success,
            response.Message
        );
    }
}
