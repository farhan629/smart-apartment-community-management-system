using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories;

public class ComplaintProgressLogRepository : IComplaintProgressLogRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ComplaintProgressLogRepository> _logger;

    public ComplaintProgressLogRepository(
        AppDbContext context,
        ILogger<ComplaintProgressLogRepository> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ComplaintProgressLog>> GetByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    )
    {
        return await _context
            .ComplaintProgressLogs.AsNoTracking()
            .Where(p => p.ComplaintId == complaintId && p.IsActive)
            .Select(p => new ComplaintProgressLog
            {
                Id = p.Id,
                ComplaintId = p.ComplaintId,
                ChangedBy = p.ChangedBy,
                StatusId = p.StatusId,
                Status = p.Status,
                Remarks = p.Remarks,
                ChangedDate = p.ChangedDate,
                CreatedAt = p.CreatedAt,
            })
            .OrderBy(p => p.ChangedDate)
            .ToListAsync(ct);
    }

    public async Task<ComplaintProgressLog> AddAsync(
        ComplaintProgressLog log,
        CancellationToken ct = default
    )
    {
        await _context.ComplaintProgressLogs.AddAsync(log, ct);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation(
            "Progress log {LogId} appended for complaint {ComplaintId}",
            log.Id,
            log.ComplaintId
        );
        return log;
    }
}
