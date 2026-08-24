using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visits.Commands;
using ResidentVisitorService.Application.Features.Visits.DTOs;
using ResidentVisitorService.Application.Features.Visits.Queries;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ResidentVisitorService.API.Controllers;

/// <summary>
/// Manages visits — register, list, update, cancel, approve, reject, check-in, and check-out.
/// </summary>
[ApiController]
[Route("api/visits")]
[Authorize]
public class VisitController : ControllerBase
{
    private readonly IMediator _mediator;

    public VisitController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns a paginated list of visits. Pass an <c>id</c> query param to fetch a single record.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize(PermissionConst.VISIT_VIEW)]
    public async Task<IActionResult> GetVisits(
        [FromQuery] Guid? id,
        [FromQuery] Guid? visitorId,
        [FromQuery] Guid? hostUserId,
        [FromQuery] Guid? flatId,
        [FromQuery] string? status,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] string sortBy = ResidentVisitorConstants.Pagination.DefaultSortBy,
        [FromQuery] string sortOrder = ResidentVisitorConstants.Pagination.DefaultSortOrder,
        [FromQuery] int page = PaginationConstants.DefaultPageNumber,
        [FromQuery] int limit = PaginationConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetVisitsQuery
        {
            Id = id,
            VisitorId = visitorId,
            HostUserId = hostUserId,
            FlatId = flatId,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            Limit = limit,
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.Pagination.TotalCount == 0)
        {
            return NoContent();
        }

        return Ok(result);
    }

    /// <summary>
    /// Registers a new visit. Supply either an existing <c>visitorId</c> or inline visitor details.
    /// </summary>
    [HttpPost]
    [PermissionAuthorize(PermissionConst.VISIT_REGISTER)]
    public async Task<IActionResult> CreateVisit(
        [FromBody] CreateVisitRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new CreateVisitCommand { Request = request },
            cancellationToken
        );
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Updates a PENDING visit's dates or purpose.
    /// </summary>
    [HttpPut]
    [PermissionAuthorize(PermissionConst.VISIT_MANAGE)]
    public async Task<IActionResult> UpdateVisit(
        [FromQuery] Guid id,
        [FromBody] UpdateVisitRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new UpdateVisitCommand { Id = id, Request = request },
            cancellationToken
        );

        return Ok(result);
    }

    /// <summary>
    /// Cancels a visit (soft delete). Only PENDING or APPROVED visits can be cancelled.
    /// </summary>
    [HttpDelete]
    [PermissionAuthorize(PermissionConst.VISIT_MANAGE)]
    public async Task<IActionResult> CancelVisit(
        [FromQuery] Guid id,
        CancellationToken cancellationToken
    )
    {
        await _mediator.Send(new CancelVisitCommand { Id = id }, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Approves a PENDING visit request.
    /// </summary>
    [HttpPatch("{id:guid}/approve")]
    [PermissionAuthorize(PermissionConst.VISIT_APPROVE)]
    public async Task<IActionResult> ApproveVisit(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(new ApproveVisitCommand { Id = id }, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Rejects a PENDING visit request.
    /// </summary>
    [HttpPatch("{id:guid}/reject")]
    [PermissionAuthorize(PermissionConst.VISIT_APPROVE)]
    public async Task<IActionResult> RejectVisit(
        [FromRoute] Guid id,
        [FromBody] RejectVisitRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new RejectVisitCommand { Id = id, Request = request },
            cancellationToken
        );

        return Ok(result);
    }

    [HttpPatch("{id:guid}/check-in")]
    [PermissionAuthorize(PermissionConst.VISIT_CHECKIN)]
    public async Task<IActionResult> CheckIn(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new CheckInVisitHardCodeCommand { Id = id },
            cancellationToken
        );
        return Ok(result);
    }

    [HttpPatch("{id:guid}/check-out")]
    [PermissionAuthorize(PermissionConst.VISIT_CHECKIN)]
    public async Task<IActionResult> CheckOut(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new CheckOutVisitHardCodeCommand { Id = id },
            cancellationToken
        );
        return Ok(result);
    }
}
