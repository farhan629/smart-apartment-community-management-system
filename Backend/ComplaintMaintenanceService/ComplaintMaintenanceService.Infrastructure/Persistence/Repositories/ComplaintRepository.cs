using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Reports.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories;

public class ComplaintRepository : IComplaintRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ComplaintRepository> _logger;

    public ComplaintRepository(AppDbContext context, ILogger<ComplaintRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Complaint> AddAsync(Complaint complaint, CancellationToken ct = default)
    {
        await _context.Complaints.AddAsync(complaint, ct);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Complaint {ComplaintId} persisted", complaint.Id);
        return complaint;
    }

    public async Task<Complaint?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context
            .Complaints.Include(c => c.Category)
            .Include(c => c.ComplaintType)
            .Include(c => c.Priority)
            .Include(c => c.Status)
            .Include(c => c.ScheduledSlot)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<(List<Complaint> Items, int TotalCount)> GetPagedAsync(
        Guid? residentId,
        Guid? assignedStaffId,
        Guid? deniedAssignmentStatusId,
        Guid? statusId,
        Guid? priorityId,
        Guid? categoryId,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int limit,
        CancellationToken ct = default
    )
    {
        var query = _context
            .Complaints.Include(c => c.Category)
            .Include(c => c.ComplaintType)
            .Include(c => c.Priority)
            .Include(c => c.Status)
            .AsQueryable();

        if (residentId.HasValue)
            query = query.Where(c => c.ResidentId == residentId.Value);

        if (assignedStaffId.HasValue)
        {
            query = query.Where(c =>
                c.ComplaintAssignments!.Any(a =>
                    a.StaffId == assignedStaffId.Value
                    && (
                        !deniedAssignmentStatusId.HasValue
                        || a.StatusId != deniedAssignmentStatusId.Value
                    )
                )
            );
        }

        if (statusId.HasValue)
            query = query.Where(c => c.StatusId == statusId.Value);
        if (priorityId.HasValue)
            query = query.Where(c => c.PriorityId == priorityId.Value);
        if (categoryId.HasValue)
            query = query.Where(c => c.CategoryId == categoryId.Value);
        if (fromDate.HasValue)
            query = query.Where(c => c.ScheduledDate != null && c.ScheduledDate >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(c => c.ScheduledDate != null && c.ScheduledDate <= toDate.Value);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task UpdateAsync(Complaint complaint, CancellationToken ct = default)
    {
        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Complaint {ComplaintId} updated", complaint.Id);
    }

    public async Task<ReportResponseDto> GetReportDataAsync(
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct = default
    )
    {
        var query = _context
            .Complaints.Include(c => c.Status)
            .Include(c => c.Category)
            .Where(c => c.IsActive);

        if (fromDate.HasValue)
            query = query.Where(c => c.CreatedAt >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(c => c.CreatedAt <= toDate.Value);

        var complaints = await query.ToListAsync(ct);

        var byCategory = complaints
            .Where(c => c.Category != null)
            .GroupBy(c => c.Category!.Name)
            .Select(g => new CategoryReportDto { CategoryName = g.Key, ComplaintCount = g.Count() })
            .ToList();

        return new ReportResponseDto
        {
            TotalComplaints = complaints.Count,
            OpenComplaints = complaints.Count(c =>
                c.Status?.Code == ComplaintConstants.StatusCodes.Open
            ),
            AssignedComplaints = complaints.Count(c =>
                c.Status?.Code == ComplaintConstants.StatusCodes.Assigned
            ),
            InProgressComplaints = complaints.Count(c =>
                c.Status?.Code == ComplaintConstants.StatusCodes.InProgress
            ),
            ResolvedComplaints = complaints.Count(c =>
                c.Status?.Code == ComplaintConstants.StatusCodes.Resolved
            ),
            CancelledComplaints = complaints.Count(c =>
                c.Status?.Code == ComplaintConstants.StatusCodes.Cancelled
            ),
            EscalatedComplaints = complaints.Count(c =>
                c.Status?.Code == ComplaintConstants.StatusCodes.Escalated
            ),
            ByCategory = byCategory,
        };
    }
}