using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace ComplaintMaintenanceService.Application.Features.StaffAvailability.Commands;

/// <summary>
/// Command to soft-delete an availability slot by setting IsCancelled = true.
/// </summary>
public class DeleteStaffAvailabilityCommand : IRequest
{
    public Guid SlotId { get; set; }
    public Guid StaffId { get; set; }
}

/// <summary>
/// Handles DELETE /staff/availability/{slotId} - validates slot exists and is not booked,
/// then soft-deletes by setting IsCancelled=true and IsActive=false.
/// </summary>
public class DeleteStaffAvailabilityCommandHandler : IRequestHandler<DeleteStaffAvailabilityCommand>
{
    private readonly IStaffAvailabilityRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteStaffAvailabilityCommandHandler> _logger;

    public DeleteStaffAvailabilityCommandHandler(
        IStaffAvailabilityRepository repo,
        ICurrentUserService currentUser,
        ILogger<DeleteStaffAvailabilityCommandHandler> logger
    )
    {
        _repo = repo;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task Handle(DeleteStaffAvailabilityCommand command, CancellationToken ct)
    {
        var slot =
            await _repo.GetByIdAndStaffAsync(command.SlotId, command.StaffId, ct)
            ?? throw new NotFoundException(StaffAvailabilityConstants.Messages.SlotNotFound);

        if (slot.IsCancelled)
            throw new BadRequestException(StaffAvailabilityConstants.Messages.SlotAlreadyCancelled);

        if (slot.IsBooked)
            throw new BadRequestException(StaffAvailabilityConstants.Messages.SlotAlreadyBooked);

        slot.IsCancelled = true;
        slot.IsActive = false;
        slot.UpdatedAt = DateTime.UtcNow;
        slot.UpdatedBy = _currentUser.UserId;

        await _repo.UpdateAsync(slot, ct);

        _logger.LogInformation("Slot {SlotId} soft-deleted by {UserId}", slot.Id, slot.UpdatedBy);
    }
}
