using AutoMapper;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Protos;
using Grpc.Core;

namespace ComplaintMaintenanceService.API.Grpc;

/// <summary>
/// gRPC server implementation of CmsService.
/// Handles category validation and staff creation requests from IdentityService.
/// </summary>
public class CmsGrpcService : CmsService.CmsServiceBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CmsGrpcService> _logger;

    public CmsGrpcService(
        ICategoryRepository categoryRepository,
        IStaffRepository staffRepository,
        IMapper mapper,
        ILogger<CmsGrpcService> logger
    )
    {
        _categoryRepository = categoryRepository;
        _staffRepository = staffRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Returns category details by ID. Called by IdentityService before creating a staff user.
    /// </summary>
    public override async Task<GetCategoryResponse> GetCategory(
        GetCategoryRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation(
            "gRPC GetCategory called for CategoryId={CategoryId}",
            request.CategoryId
        );

        if (!Guid.TryParse(request.CategoryId, out var categoryId))
            return new GetCategoryResponse { Found = false };

        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category is null)
            return new GetCategoryResponse { Found = false };

        return _mapper.Map<GetCategoryResponse>(category);
    }

    /// <summary>
    /// Creates a Staff record in CMS DB. Called by IdentityService after user creation.
    /// </summary>
    public override async Task<CreateStaffResponse> CreateStaff(
        CreateStaffRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation("gRPC CreateStaff called for UserId={UserId}", request.UserId);

        if (
            !Guid.TryParse(request.UserId, out var userId)
            || !Guid.TryParse(request.CategoryId, out var categoryId)
        )
            return new CreateStaffResponse
            {
                Success = false,
                Message = ComplaintConstants.GrpcMessages.InvalidUserId,
            };

        var existing = await _staffRepository.GetByUserIdAsync(userId);
        if (existing is not null)
            return new CreateStaffResponse
            {
                Success = false,
                Message = ComplaintConstants.GrpcMessages.StaffCreatedSuccess,
            };

        var staff = new Staff
        {
            UserId = userId,
            CategoryId = categoryId,
            Description = request.Description,
            Details = request.Details,
            IsActive = true,
        };

        var created = await _staffRepository.AddAsync(staff);

        _logger.LogInformation("gRPC CreateStaff - Staff {StaffId} created", created.Id);

        return new CreateStaffResponse
        {
            StaffId = created.Id.ToString(),
            Success = true,
            Message = ComplaintConstants.GrpcMessages.StaffCreatedSuccess,
        };
    }
}
