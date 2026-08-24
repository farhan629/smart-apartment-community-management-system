using AutoMapper;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.Queries;

/// <summary>
/// Query to retrieve a single availability slot by slotId scoped to a staffId.
/// </summary>
public class GetStaffAvailabilityByIdQuery : IRequest<AvailabilitySlotResponseDto>
{
    public Guid SlotId { get; set; }
    public Guid StaffId { get; set; }
}

/// <summary>
/// Handles GET /staff/availability/{slotId} - fetches slot, throws NotFoundException if missing.
/// </summary>
public class GetStaffAvailabilityByIdQueryHandler
    : IRequestHandler<GetStaffAvailabilityByIdQuery, AvailabilitySlotResponseDto>
{
    private readonly IStaffAvailabilityRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStaffAvailabilityByIdQueryHandler> _logger;

    public GetStaffAvailabilityByIdQueryHandler(
        IStaffAvailabilityRepository repo,
        IMapper mapper,
        ILogger<GetStaffAvailabilityByIdQueryHandler> logger
    )
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AvailabilitySlotResponseDto> Handle(
        GetStaffAvailabilityByIdQuery query,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            "GetStaffAvailabilityByIdQuery - slotId={SlotId} staffId={StaffId}",
            query.SlotId,
            query.StaffId
        );

        var slot =
            await _repo.GetByIdAndStaffAsync(query.SlotId, query.StaffId, ct)
            ?? throw new NotFoundException(StaffAvailabilityConstants.Messages.SlotNotFound);

        return _mapper.Map<AvailabilitySlotResponseDto>(slot);
    }
}
