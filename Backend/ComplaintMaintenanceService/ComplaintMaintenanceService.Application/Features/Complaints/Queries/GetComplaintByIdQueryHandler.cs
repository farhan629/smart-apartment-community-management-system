using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Queries;

public class GetComplaintByIdQuery : IRequest<ComplaintDetailDto>
{
    public Guid ComplaintId { get; set; }
    public bool IsResident { get; set; }
}

public class GetComplaintByIdQueryHandler
    : IRequestHandler<GetComplaintByIdQuery, ComplaintDetailDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly ILogger<GetComplaintByIdQueryHandler> _logger;

    public GetComplaintByIdQueryHandler(
        IComplaintRepository complaintRepo,
        ILogger<GetComplaintByIdQueryHandler> logger
    )
    {
        _complaintRepo = complaintRepo;
        _logger = logger;
    }

    public async Task<ComplaintDetailDto> Handle(GetComplaintByIdQuery query, CancellationToken ct)
    {
        var complaint =
            await _complaintRepo.GetByIdAsync(query.ComplaintId, ct)
            ?? throw new KeyNotFoundException(ComplaintConstants.Messages.ComplaintNotFound);

        _logger.LogInformation("Complaint {ComplaintId} fetched", complaint.Id);

        return new ComplaintDetailDto
        {
            ComplaintId = complaint.Id,
            ResidentId = complaint.ResidentId,
            Description = complaint.Description,
            ComplaintType = complaint.ComplaintType?.DisplayName ?? string.Empty,
            Category = complaint.Category?.Name ?? string.Empty,
            CategoryId = complaint.CategoryId,
            CategoryImg = complaint.Category?.Img,
            Priority = complaint.Priority?.DisplayName ?? string.Empty,
            Status = complaint.Status?.DisplayName ?? string.Empty,
            ScheduledDate = complaint.ScheduledDate?.ToString(
                ComplaintConstants.DateFormats.OutputDate
            ),
            ScheduledTime = complaint.ScheduledTime?.ToString(
                ComplaintConstants.DateFormats.OutputTime
            ),
            ScheduledSlotId = complaint.ScheduledSlotId,
            CancelledAt = complaint.CancelledAt,
            CancellationReason = complaint.CancellationReason,
            CreatedAt = complaint.CreatedAt,
            UpdatedAt = complaint.UpdatedAt,
        };
    }
}
