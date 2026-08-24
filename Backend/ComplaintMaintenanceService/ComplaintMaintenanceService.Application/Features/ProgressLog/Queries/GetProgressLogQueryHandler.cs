using ComplaintMaintenanceService.Application.Features.ProgressLog.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;

namespace ComplaintMaintenanceService.Application.Features.ProgressLog.Queries;

public class GetProgressLogQuery : IRequest<List<ProgressLogEntryDto>>
{
    public Guid ComplaintId { get; set; }
}

public class GetProgressLogQueryHandler
    : IRequestHandler<GetProgressLogQuery, List<ProgressLogEntryDto>>
{
    private readonly IComplaintProgressLogRepository _progressRepo;

    public GetProgressLogQueryHandler(IComplaintProgressLogRepository progressRepo)
    {
        _progressRepo = progressRepo;
    }

    public async Task<List<ProgressLogEntryDto>> Handle(
        GetProgressLogQuery query,
        CancellationToken ct
    )
    {
        var logs = await _progressRepo.GetByComplaintIdAsync(query.ComplaintId, ct);

        return logs.Select(l => new ProgressLogEntryDto
            {
                LogId = l.Id,
                ComplaintId = l.ComplaintId,
                ChangedBy = l.ChangedBy,
                Status = l.Status?.DisplayName ?? string.Empty,
                Remarks = l.Remarks,
                ChangedDate = l.ChangedDate,
            })
            .ToList();
    }
}
