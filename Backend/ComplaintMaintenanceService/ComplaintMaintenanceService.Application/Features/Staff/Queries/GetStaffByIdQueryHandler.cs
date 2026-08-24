using AutoMapper;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Staff.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Staff.Queries;

/// <summary>
/// Query to retrieve a single staff member full profile by Staff primary key.
/// </summary>
public class GetStaffByIdQuery : IRequest<StaffResponseDto>
{
    public Guid StaffId { get; set; }
}

/// <summary>
/// Fetches Staff by ID, throws NotFoundException if missing, maps to StaffResponseDto via AutoMapper.
/// </summary>
public class GetStaffByIdQueryHandler : IRequestHandler<GetStaffByIdQuery, StaffResponseDto>
{
    private readonly IStaffRepository _staffRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStaffByIdQueryHandler> _logger;

    public GetStaffByIdQueryHandler(
        IStaffRepository staffRepo,
        IMapper mapper,
        ILogger<GetStaffByIdQueryHandler> logger
    )
    {
        _staffRepo = staffRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<StaffResponseDto> Handle(GetStaffByIdQuery query, CancellationToken ct)
    {
        _logger.LogInformation("GetStaffByIdQuery - fetching staff {StaffId}", query.StaffId);

        var staff =
            await _staffRepo.GetByIdAsync(query.StaffId, ct)
            ?? throw new NotFoundException(StaffConstants.Messages.StaffNotFound);

        return _mapper.Map<StaffResponseDto>(staff);
    }
}
