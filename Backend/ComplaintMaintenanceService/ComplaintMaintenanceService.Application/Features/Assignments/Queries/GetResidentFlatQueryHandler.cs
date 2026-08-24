using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using MediatR;
using Shared.SharedLibrary.Services;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Queries;

public record GetResidentFlatQuery(Guid? AssignmentId, Guid ComplaintId)
    : IRequest<ResidentFlatResponseDto>;

public class GetResidentFlatQueryHandler
    : IRequestHandler<GetResidentFlatQuery, ResidentFlatResponseDto>
{
    private readonly IComplaintAssignmentRepository _assignmentRepository;
    private readonly IComplaintRepository _complaintRepository;
    private readonly IFlatLookupGrpcClient _flatLookupGrpcClient;
    private readonly ICurrentUserService _currentUserService;

    public GetResidentFlatQueryHandler(
        IComplaintAssignmentRepository assignmentRepository,
        IComplaintRepository complaintRepository,
        IFlatLookupGrpcClient flatLookupGrpcClient,
        ICurrentUserService currentUserService
    )
    {
        _assignmentRepository = assignmentRepository;
        _complaintRepository = complaintRepository;
        _flatLookupGrpcClient = flatLookupGrpcClient;
        _currentUserService = currentUserService;
    }

    public async Task<ResidentFlatResponseDto> Handle(
        GetResidentFlatQuery request,
        CancellationToken cancellationToken
    )
    {
        var isAdmin = _currentUserService.RoleId == ComplaintConstants.RoleIds.Admin;

        if (!isAdmin)
        {
            if (request.AssignmentId is null || request.AssignmentId == Guid.Empty)
                throw new InvalidOperationException(
                    ComplaintConstants.AssignmentMessages.AssignmentIdRequired
                );

            var assignment =
                await _assignmentRepository.GetByIdAsync(request.AssignmentId.Value)
                ?? throw new KeyNotFoundException(
                    ComplaintConstants.AssignmentMessages.AssignmentNotFound
                );

            if (assignment.ComplaintId != request.ComplaintId)
                throw new InvalidOperationException(
                    ComplaintConstants.AssignmentMessages.InvalidAssignment
                );
        }

        var complaint =
            await _complaintRepository.GetByIdAsync(request.ComplaintId)
            ?? throw new KeyNotFoundException(ComplaintConstants.Messages.ComplaintNotFound);

        var flatInfo =
            await _flatLookupGrpcClient.GetFlatByUserIdAsync(complaint.ResidentId)
            ?? throw new KeyNotFoundException(ComplaintConstants.FlatLookupMessages.FlatNotFound);

        return new ResidentFlatResponseDto(
            flatInfo.FlatId,
            flatInfo.ResidentName,
            flatInfo.ResidentEmail,
            flatInfo.Block,
            flatInfo.FlatNumber
        );
    }
}
