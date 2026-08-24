using ComplaintMaintenanceService.Application.Features.Reports.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Application.Features.Reports.Queries;

public class GetReportQuery : IRequest<ReportResponseDto>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetReportQueryHandler : IRequestHandler<GetReportQuery, ReportResponseDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IComplaintEscalationRepository _escalationRepo;
    private readonly ILogger<GetReportQueryHandler> _logger;

    public GetReportQueryHandler(
        IComplaintRepository complaintRepo,
        IComplaintEscalationRepository escalationRepo,
        ILogger<GetReportQueryHandler> logger
    )
    {
        _complaintRepo = complaintRepo;
        _escalationRepo = escalationRepo;
        _logger = logger;
    }

    public async Task<ReportResponseDto> Handle(GetReportQuery query, CancellationToken ct)
    {
        var report = await _complaintRepo.GetReportDataAsync(query.FromDate, query.ToDate, ct);
        report.TotalEscalations = await _escalationRepo.GetTotalCountAsync(ct);

        _logger.LogInformation(
            "Report generated for range {From} - {To}",
            query.FromDate,
            query.ToDate
        );

        return report;
    }
}
