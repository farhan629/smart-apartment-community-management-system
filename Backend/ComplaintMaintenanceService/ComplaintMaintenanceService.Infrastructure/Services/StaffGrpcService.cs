using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using StaffGrpc;

namespace ComplaintMaintenanceService.Infrastructure.Services;

/// <summary>
/// gRPC server implementation for staff profile creation.
/// Receives CreateStaff calls from IdentityService after a staff user
/// is registered. Validates the categoryId exists locally, then persists
/// the Staff row so ComplaintMaintenanceService can assign complaints to them.
/// </summary>
public class StaffGrpcService : StaffService.StaffServiceBase
{
    private readonly IStaffRepository _staffRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly ILogger<StaffGrpcService> _logger;

    public StaffGrpcService(
        IStaffRepository staffRepo,
        ICategoryRepository categoryRepo,
        ILogger<StaffGrpcService> logger
    )
    {
        _staffRepo = staffRepo;
        _categoryRepo = categoryRepo;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateStaff gRPC call from IdentityService.
    /// Validates userId and categoryId, guards against duplicate profiles,
    /// then saves the Staff record and returns the new staff ID.
    /// </summary>
    public override async Task<CreateStaffResponse> CreateStaff(
        CreateStaffRequest request,
        ServerCallContext context
    )
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                _logger.LogWarning(
                    "gRPC CreateStaff - {Reason}",
                    ComplaintConstants.GrpcMessages.InvalidUserId
                );
                throw new RpcException(
                    new Status(
                        StatusCode.InvalidArgument,
                        ComplaintConstants.GrpcMessages.InvalidUserId
                    )
                );
            }

            if (!Guid.TryParse(request.CategoryId, out var categoryId))
            {
                _logger.LogWarning(
                    "gRPC CreateStaff - {Reason}",
                    ComplaintConstants.GrpcMessages.InvalidCategoryId
                );
                throw new RpcException(
                    new Status(
                        StatusCode.InvalidArgument,
                        ComplaintConstants.GrpcMessages.InvalidCategoryId
                    )
                );
            }

            var existing = await _staffRepo.GetByUserIdAsync(userId);
            if (existing is not null)
            {
                _logger.LogWarning(
                    "gRPC CreateStaff - Staff profile already exists for userId {UserId}",
                    userId
                );
                return new CreateStaffResponse
                {
                    Success = true,
                    StaffId = existing.Id.ToString(),
                    Message = ComplaintConstants.GrpcMessages.StaffCreatedSuccess,
                };
            }

            var category = await _categoryRepo.GetByIdAsync(categoryId);
            if (category is null)
            {
                _logger.LogWarning(
                    "gRPC CreateStaff - {Reason} CategoryId={CategoryId}",
                    ComplaintConstants.GrpcMessages.CategoryNotFound,
                    categoryId
                );
                throw new RpcException(
                    new Status(
                        StatusCode.NotFound,
                        ComplaintConstants.GrpcMessages.CategoryNotFound
                    )
                );
            }

            var now = DateTime.UtcNow;
            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = categoryId,
                Description = string.Empty,
                Details = string.Empty,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = userId,
                UpdatedBy = userId,
            };

            await _staffRepo.AddAsync(staff);

            _logger.LogInformation(
                "gRPC CreateStaff - Staff {StaffId} created for userId {UserId} in category {CategoryId}",
                staff.Id,
                userId,
                categoryId
            );

            return new CreateStaffResponse
            {
                Success = true,
                StaffId = staff.Id.ToString(),
                Message = ComplaintConstants.GrpcMessages.StaffCreatedSuccess,
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "gRPC CreateStaff - {Reason} userId={UserId}",
                ComplaintConstants.GrpcMessages.InternalError,
                request.UserId
            );
            throw new RpcException(
                new Status(StatusCode.Internal, ComplaintConstants.GrpcMessages.InternalError)
            );
        }
    }
}
