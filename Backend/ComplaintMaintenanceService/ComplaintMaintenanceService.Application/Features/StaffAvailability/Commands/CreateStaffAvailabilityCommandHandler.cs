using AutoMapper;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;
using DomainEntities = ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.Commands;

public class CreateStaffAvailabilityCommand : IRequest<List<AvailabilitySlotResponseDto>>
{
    public Guid StaffId { get; set; }
    public CreateAvailabilityRequestDto Request { get; set; } = null!;
}

public class CreateStaffAvailabilityCommandHandler
    : IRequestHandler<CreateStaffAvailabilityCommand, List<AvailabilitySlotResponseDto>>
{
    private readonly IStaffAvailabilityRepository _availRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateStaffAvailabilityCommandHandler> _logger;

    public CreateStaffAvailabilityCommandHandler(
        IStaffAvailabilityRepository availRepo,
        IStaffRepository staffRepo,
        ICurrentUserService currentUser,
        IMapper mapper,
        ILogger<CreateStaffAvailabilityCommandHandler> logger
    )
    {
        _availRepo = availRepo;
        _staffRepo = staffRepo;
        _currentUser = currentUser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<AvailabilitySlotResponseDto>> Handle(
        CreateStaffAvailabilityCommand command,
        CancellationToken ct
    )
    {
        var staff =
            await _staffRepo.GetByIdAsync(command.StaffId, ct)
            ?? throw new NotFoundException(StaffAvailabilityConstants.Messages.StaffNotFound);

        var now = DateTime.UtcNow;
        var createdBy = _currentUser.UserId;

        var slots = command
            .Request.Slots.Select(s =>
            {
                var date = DateTime.SpecifyKind(DateTime.Parse(s.Date).Date, DateTimeKind.Utc);
                var startTime = ParseTime(s.StartTime);
                var endTime = ParseTime(s.EndTime);

                return new DomainEntities.StaffAvailability
                {
                    Id = Guid.NewGuid(),
                    StaffId = staff.Id,
                    AvailableDate = date,
                    SlotStartTime = startTime,
                    SlotEndTime = endTime,
                    IsBooked = false,
                    IsCancelled = false,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    Staff = staff,
                };
            })
            .ToList();

        await _availRepo.AddRangeAsync(slots, ct);

        _logger.LogInformation(
            "Created {Count} availability slots for staff {StaffId}",
            slots.Count,
            staff.Id
        );

        return _mapper.Map<List<AvailabilitySlotResponseDto>>(slots);
    }

    private static TimeSpan ParseTime(string time)
    {
        if (TimeSpan.TryParse(time, out var ts))
            return ts;

        if (
            DateTime.TryParseExact(
                time.Trim(),
                new[] { "h:mmtt", "hh:mmtt", "h:mm tt", "hh:mm tt" },
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var dt
            )
        )
            return dt.TimeOfDay;

        throw new ArgumentException($"Invalid time format: '{time}'. Use HH:mm or hh:mmAM/PM.");
    }
}
