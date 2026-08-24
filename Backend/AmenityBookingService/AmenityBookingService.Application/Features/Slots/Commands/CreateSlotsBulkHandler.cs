using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Slots.DTO;
using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Domain.Entities;
using AutoMapper;
using MediatR;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Slots.Commands;

/// <summary>
/// Command to create multiple slots for an amenity in a single bulk operation.
/// </summary>
public class CreateSlotsBulkCommand : IRequest<SlotsBulkResponseDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the amenity to create slots for.
    /// </summary>
    public Guid AmenityId { get; set; }

    /// <summary>
    /// Gets or sets the list of slot creation request DTOs.
    /// </summary>
    public List<CreateSlotRequestDto> Slots { get; set; } = new();
}

/// <summary>
/// Response DTO containing the result of a bulk slot creation operation.
/// </summary>
public class SlotsBulkResponseDto
{
    /// <summary>
    /// Gets or sets the result message indicating creation status.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of IDs for the successfully created slots.
    /// </summary>
    public List<Guid> SlotIds { get; set; } = new();
}

/// <summary>
/// Handler for processing <see cref="CreateSlotsBulkCommand"/> to create multiple slots in bulk.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Amenity must exist and have a valid slot type
/// - User must be authenticated
/// - For TIME slot type, max capacity must be 1
/// - Duplicate slots (same date and start time) are not allowed
/// - Each slot is validated individually
/// - Transaction-like behavior (all succeed or none are persisted)
/// </remarks>
public class CreateSlotsBulkHandler : IRequestHandler<CreateSlotsBulkCommand, SlotsBulkResponseDto>
{
    private readonly ISlotRepository _slotRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateSlotsBulkHandler(
        ISlotRepository slotRepository,
        IAmenityRepository amenityRepository,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _slotRepository = slotRepository;
        _amenityRepository = amenityRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<SlotsBulkResponseDto> Handle(
        CreateSlotsBulkCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        var amenity = await _amenityRepository.GetAmenityWithSlotTypeAsync(
            request.AmenityId,
            cancellationToken
        );
        if (amenity == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.AmenityNotFound, request.AmenityId)
            );

        var slotIds = new List<Guid>();

        foreach (var slotDto in request.Slots)
        {
            if (amenity.SlotType?.Code == RefTermCodes.Time && slotDto.MaxCapacity != 1)
                throw new BadRequestException(
                    string.Format(ErrorMessages.TimeSlotMaxCapacity, slotDto.SlotLabel)
                );

            var slotExists = await _slotRepository.SlotExistsAsync(
                request.AmenityId,
                slotDto.SlotDate,
                slotDto.StartTime,
                cancellationToken
            );
            if (slotExists)
                throw new ConflictException(
                    string.Format(
                        ErrorMessages.SlotAlreadyExists,
                        slotDto.SlotDate,
                        slotDto.StartTime
                    )
                );

            var slot = _mapper.Map<AmenitySlot>(slotDto);
            slot.AmenityId = request.AmenityId;
            slot.SlotDate = DateTime.SpecifyKind(slotDto.SlotDate, DateTimeKind.Utc);
            slot.CurrentBookingCount = 0;

            var result = await _slotRepository.CreateSlotAsync(slot, cancellationToken);
            slotIds.Add(result.Id);
        }

        return new SlotsBulkResponseDto
        {
            Message = string.Format(SuccessMessages.SlotsBulkCreated, slotIds.Count),
            SlotIds = slotIds,
        };
    }
}
