using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Slots.Commands;

/// <summary>
/// Command to update an existing slot. All properties are optional for partial updates.
/// </summary>
public class UpdateSlotCommand : IRequest<MessageResponseDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the slot to update.
    /// </summary>
    public Guid SlotId { get; set; }

    /// <summary>
    /// Gets or sets the updated slot label. Optional.
    /// </summary>
    public string? SlotLabel { get; set; }

    /// <summary>
    /// Gets or sets the updated start time. Optional.
    /// </summary>
    public TimeSpan? StartTime { get; set; }

    /// <summary>
    /// Gets or sets the updated end time. Optional.
    /// </summary>
    public TimeSpan? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the updated maximum capacity. Optional.
    /// </summary>
    public int? MaxCapacity { get; set; }
}

/// <summary>
/// Handler for processing <see cref="UpdateSlotCommand"/> to update an existing slot.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Slot must exist
/// - Amenity associated with the slot must exist
/// - User must be authenticated
/// - Only provided fields are updated (partial update)
/// - Max capacity cannot be reduced below current booking count
/// - For TIME slot type, max capacity must be 1
/// - End time must be after start time
/// </remarks>
public class UpdateSlotHandler : IRequestHandler<UpdateSlotCommand, MessageResponseDto>
{
    private readonly ISlotRepository _slotRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateSlotHandler(
        ISlotRepository slotRepository,
        IAmenityRepository amenityRepository,
        ICurrentUserService currentUserService
    )
    {
        _slotRepository = slotRepository;
        _amenityRepository = amenityRepository;
        _currentUserService = currentUserService;
    }

    public async Task<MessageResponseDto> Handle(
        UpdateSlotCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        var slot = await _slotRepository.GetSlotByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.SlotTypeNotFound, request.SlotId)
            );

        var amenity = await _amenityRepository.GetAmenityWithSlotTypeAsync(
            slot.AmenityId,
            cancellationToken
        );
        if (amenity == null)
            throw new NotFoundException(ErrorMessages.AmenitiesNotFound);

        if (request.MaxCapacity.HasValue)
        {
            var currentBookings = await _slotRepository.GetCurrentBookingsCountForSlotAsync(
                request.SlotId,
                cancellationToken
            );

            if (request.MaxCapacity.Value < currentBookings)
                throw new BadRequestException(
                    string.Format(ErrorMessages.CapacityBelowBookings, currentBookings)
                );

            if (amenity.SlotType?.Code == RefTermCodes.Time && request.MaxCapacity.Value != 1)
                throw new BadRequestException(ErrorMessages.TimeSlotMaxCapacit);
        }

        if (request.StartTime.HasValue && request.EndTime.HasValue)
        {
            if (request.EndTime.Value <= request.StartTime.Value)
                throw new BadRequestException(ErrorMessages.EndTimeValidation);
        }

        if (!string.IsNullOrEmpty(request.SlotLabel))
            slot.SlotLabel = request.SlotLabel;

        if (request.StartTime.HasValue)
            slot.StartTime = request.StartTime.Value;

        if (request.EndTime.HasValue)
            slot.EndTime = request.EndTime.Value;

        if (request.MaxCapacity.HasValue)
            slot.MaxCapacity = request.MaxCapacity.Value;

        await _slotRepository.UpdateSlotAsync(slot, cancellationToken);

        return new MessageResponseDto { Message = SuccessMessages.SlotUpdateMessage };
    }
}
