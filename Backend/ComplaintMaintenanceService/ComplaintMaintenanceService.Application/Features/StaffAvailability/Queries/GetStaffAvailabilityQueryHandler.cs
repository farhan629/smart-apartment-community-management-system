using AutoMapper;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.Queries;

/// <summary>
/// Query to retrieve availability slots with optional filters.
/// </summary>
public class GetStaffAvailabilityQuery : IRequest<List<AvailabilitySlotResponseDto>>
{
    public Guid? StaffId { get; set; }
    public DateTime? Date { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsBooked { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
}

/// <summary>
/// Handles GET /staff/availability - applies filters via repository and maps results via AutoMapper.
/// </summary>
public class GetStaffAvailabilityQueryHandler
    : IRequestHandler<GetStaffAvailabilityQuery, List<AvailabilitySlotResponseDto>>
{
    private readonly IStaffAvailabilityRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStaffAvailabilityQueryHandler> _logger;

    public GetStaffAvailabilityQueryHandler(
        IStaffAvailabilityRepository repo,
        IMapper mapper,
        ILogger<GetStaffAvailabilityQueryHandler> logger
    )
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<AvailabilitySlotResponseDto>> Handle(
        GetStaffAvailabilityQuery query,
        CancellationToken ct
    )
    {
        _logger.LogInformation("GetStaffAvailabilityQuery - applying filters");

        var slots = await _repo.GetFilteredAsync(
            query.StaffId,
            query.Date,
            query.CategoryId,
            query.IsBooked,
            query.FromDate,
            query.ToDate,
            query.StartTime,
            query.EndTime,
            ct
        );

        return _mapper.Map<List<AvailabilitySlotResponseDto>>(slots);
    }
}
