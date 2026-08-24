using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Queries;

public class GetAssignmentHistoryQuery : IRequest<List<AssignmentResponseDto>>
{
    public Guid ComplaintId { get; set; }
}

public class GetAssignmentHistoryQueryHandler
    : IRequestHandler<GetAssignmentHistoryQuery, List<AssignmentResponseDto>>
{
    private readonly IComplaintAssignmentRepository _assignmentRepo;
    private readonly ILogger<GetAssignmentHistoryQueryHandler> _logger;

    public GetAssignmentHistoryQueryHandler(
        IComplaintAssignmentRepository assignmentRepo,
        ILogger<GetAssignmentHistoryQueryHandler> logger
    )
    {
        _assignmentRepo = assignmentRepo;
        _logger = logger;
    }

    public async Task<List<AssignmentResponseDto>> Handle(
        GetAssignmentHistoryQuery query,
        CancellationToken ct
    )
    {
        var assignments = await _assignmentRepo.GetByComplaintIdAsync(query.ComplaintId, ct);
        _logger.LogInformation(
            "Assignment history fetched for complaint {ComplaintId}",
            query.ComplaintId
        );
        return assignments
            .Select(a => new AssignmentResponseDto
            {
                AssignmentId = a.Id,
                ComplaintId = a.ComplaintId,
                StaffId = a.StaffId,
                StaffName = a.Staff?.Description ?? string.Empty,
                Status = a.Status?.DisplayName ?? string.Empty,
                AssignedDate = a.AssignedDate,
                DueDate = a.DueDate,
                AcceptedDate = a.AcceptedDate,
                DeniedDate = a.DeniedDate,
                DenialReason = a.DenialReason,
                AssignedBy = a.AssignedBy,
            })
            .ToList();
    }
}
