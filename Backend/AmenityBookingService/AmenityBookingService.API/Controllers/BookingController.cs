using AmenityBookingService.Application.Features.Bookings.Commands;
using AmenityBookingService.Application.Features.Bookings.DTO;
using AmenityBookingService.Application.Features.Bookings.Queries;
using AmenityBookingService.Application.Features.Reports.DTO;
using AmenityBookingService.Application.Features.Reports.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.API.Controllers;

/// <summary>
/// Provides endpoints for managing amenity bookings, including creating,
/// viewing, cancelling bookings, and generating booking reports.
/// </summary>
[ApiController]
[Route("api/booking")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingController"/> class.
    /// </summary>
    /// <param name="mediator">Mediator instance used to dispatch commands and queries.</param>
    /// <param name="mapper">Mapper instance used to map DTOs to commands.</param>
    public BookingController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves the authenticated user's booking history with optional filtering and pagination.
    /// </summary>
    /// <param name="status">Optional booking status used to filter the results.</param>
    /// <param name="fromDate">Optional start date used to filter bookings.</param>
    /// <param name="toDate">Optional end date used to filter bookings.</param>
    /// <param name="pageNumber">The page number for paginated results.</param>
    /// <param name="pageSize">The number of records to return per page.</param>
    /// <returns>A paginated list of the user's bookings.</returns>
    [HttpGet]
    [PermissionAuthorize(PermissionConst.SLOT_APPLY)]
    public async Task<ActionResult<BookingListResponseDto>> GetMyBookings(
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = PaginationConstants.DefaultPageNumber,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize
    )
    {
        var query = new GetBookingHistoryQuery
        {
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new amenity booking for the authenticated user.
    /// </summary>
    /// <param name="dto">The booking request details.</param>
    /// <returns>The unique identifier of the newly created booking.</returns>
    [HttpPost]
    [PermissionAuthorize(PermissionConst.SLOT_APPLY)]
    public async Task<ActionResult<IdResponseDto>> CreateBooking(
        [FromBody] CreateBookingRequestDto dto
    )
    {
        var command = _mapper.Map<CreateBookingCommand>(dto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMyBookings), null, result);
    }

    /// <summary>
    /// Cancels an existing booking.
    /// </summary>
    /// <param name="bookingId">The unique identifier of the booking to cancel.</param>
    /// <param name="cancellationReason">Optional reason for cancellation.</param>
    /// <returns>A response indicating whether the booking was cancelled successfully.</returns>
    [HttpDelete("{bookingId}")]
    [PermissionAuthorize(PermissionConst.SLOT_APPLY)]
    public async Task<ActionResult<MessageResponseDto>> CancelBooking(
        Guid bookingId,
        [FromQuery] string? cancellationReason = null
    )
    {
        var command = new CancelBookingCommand
        {
            BookingId = bookingId,
            CancellationReason = cancellationReason,
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated booking report with optional filtering by amenity,
    /// slot type, and booking date range.
    /// </summary>
    /// <param name="amenityId">Optional amenity identifier used to filter the report.</param>
    /// <param name="slotType">Optional slot type used to filter the report.</param>
    /// <param name="fromDate">Optional start date used to filter the report.</param>
    /// <param name="toDate">Optional end date used to filter the report.</param>
    /// <param name="pageNumber">The page number for paginated results.</param>
    /// <param name="pageSize">The number of records to return per page.</param>
    /// <returns>A paginated booking report.</returns>
    [HttpGet("report")]
    [PermissionAuthorize(PermissionConst.AMENITY_MANAGE)]
    public async Task<ActionResult<ReportResponseDto>> GetBookingReport(
        [FromQuery] Guid? amenityId = null,
        [FromQuery] string? slotType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = PaginationConstants.DefaultPageNumber,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize
    )
    {
        var query = new GetBookingReportQuery
        {
            AmenityId = amenityId,
            SlotType = slotType,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
