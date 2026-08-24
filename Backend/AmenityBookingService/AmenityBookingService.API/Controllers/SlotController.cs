using AmenityBookingService.Application.Features.Slots.Commands;
using AmenityBookingService.Application.Features.Slots.DTO;
using AmenityBookingService.Application.Features.Slots.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.API.Controllers;

/// <summary>
/// Controller for managing slot CRUD operations, including bulk creation and availability checks.
/// </summary>
[ApiController]
[Route("api/slot")]
[Authorize]
public class SlotController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlotController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR request mediator.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public SlotController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a paginated list of slots for a specific amenity.
    /// </summary>
    /// <param name="amenityId">The unique identifier of the amenity.</param>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The page size (default: 10).</param>
    /// <returns>A paginated list of slot response DTOs.</returns>
    [HttpGet("/api/amenities/{amenityId}/slots")]
    [PermissionAuthorize(PermissionConst.SLOT_VIEW)]
    public async Task<ActionResult<SlotListResponseDto>> GetSlots(
        Guid amenityId,
        [FromQuery] int pageNumber = PaginationConstants.DefaultPageNumber,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize
    )
    {
        var query = new GetSlotsQuery
        {
            AmenityId = amenityId,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves available slots for a specific amenity, optionally filtered by date.
    /// </summary>
    /// <param name="amenityId">The unique identifier of the amenity.</param>
    /// <param name="date">Optional date filter for available slots. If not provided, returns slots for the current date.</param>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The page size (default: 10).</param>
    /// <returns>A paginated list of available slot response DTOs.</returns>
    [HttpGet("/api/amenities/{amenityId}/slots/available")]
    [PermissionAuthorize(PermissionConst.SLOT_VIEW)]
    public async Task<ActionResult<AvailableSlotsResponseDto>> GetAvailableSlots(
        Guid amenityId,
        [FromQuery] DateTime? date = null,
        [FromQuery] int pageNumber = PaginationConstants.DefaultPageNumber,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize
    )
    {
        var query = new GetAvailableSlotsQuery
        {
            AmenityId = amenityId,
            Date = date,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates multiple slots in bulk for a specific amenity in a single operation.
    /// </summary>
    /// <param name="amenityId">The unique identifier of the amenity.</param>
    /// <param name="dto">The bulk slot creation request DTO containing the list of slots to create.</param>
    /// <returns>A response DTO containing the count of successfully created slots and any failures.</returns>
    [HttpPost("/api/amenities/{amenityId}/slots/bulk")]
    [PermissionAuthorize(PermissionConst.SLOT_MANAGE)]
    public async Task<ActionResult<SlotsBulkResponseDto>> CreateSlotsBulk(
        Guid amenityId,
        [FromBody] CreateSlotsBulkRequestDto dto
    )
    {
        var command = _mapper.Map<CreateSlotsBulkCommand>(dto);
        command.AmenityId = amenityId;
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSlots), new { amenityId }, result);
    }

    /// <summary>
    /// Updates an existing slot's details.
    /// </summary>
    /// <param name="slotId">The unique identifier of the slot to update.</param>
    /// <param name="dto">The update slot request DTO containing the new slot details.</param>
    /// <returns>A message response indicating success.</returns>
    [HttpPut("/api/slots/{slotId}")]
    [PermissionAuthorize(PermissionConst.SLOT_MANAGE)]
    public async Task<ActionResult<MessageResponseDto>> UpdateSlot(
        Guid slotId,
        [FromBody] UpdateSlotRequestDto dto
    )
    {
        var command = _mapper.Map<UpdateSlotCommand>(dto);
        command.SlotId = slotId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Soft-deletes a slot. Fails if the slot has active bookings or is already deleted.
    /// </summary>
    /// <param name="slotId">The unique identifier of the slot to delete.</param>
    /// <returns>A message response indicating success.</returns>
    [HttpDelete("/api/slots/{slotId}")]
    [PermissionAuthorize(PermissionConst.SLOT_MANAGE)]
    public async Task<ActionResult<MessageResponseDto>> DeleteSlot(Guid slotId)
    {
        var command = new DeleteSlotCommand(slotId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
