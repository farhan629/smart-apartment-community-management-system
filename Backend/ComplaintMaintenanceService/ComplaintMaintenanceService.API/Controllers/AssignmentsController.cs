using System.Security.Claims;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.Commands;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Features.Assignments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Route("api/complaints/{id:guid}/assignments")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(IMediator mediator, ILogger<AssignmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Admin assigns a complaint to staff. POST api/complaints/{id}/assign</summary>
    [HttpPost("/api/complaints/{id:guid}/assign")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_ASSIGN)]
    public async Task<IActionResult> AssignComplaint(
        Guid id,
        [FromBody] AssignComplaintRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation("AssignComplaint invoked for {ComplaintId}", id);
        var result = await _mediator.Send(
            new AssignComplaintCommand
            {
                ComplaintId = id,
                AssignedBy = CurrentUserId,
                Request = request,
            },
            ct
        );
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Admin or Staff views assignment history. GET api/complaints/{id}/assignments</summary>
    [HttpGet]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetAssignmentHistory(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("GetAssignmentHistory invoked for {ComplaintId}", id);
        var result = await _mediator.Send(new GetAssignmentHistoryQuery { ComplaintId = id }, ct);
        return Ok(result);
    }

    [HttpGet("resident-flat")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetResidentFlat(Guid id, [FromQuery] Guid assignmentId)
    {
        var result = await _mediator.Send(new GetResidentFlatQuery(assignmentId, id));
        return Ok(result);
    }

    /// <summary>Staff accepts their assignment. PATCH api/complaints/{id}/assignments/{assignmentId}/accept</summary>
    [HttpPatch("{assignmentId:guid}/accept")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_MANAGE)]
    public async Task<IActionResult> AcceptAssignment(
        Guid id,
        Guid assignmentId,
        CancellationToken ct
    )
    {
        _logger.LogInformation("AcceptAssignment invoked for {AssignmentId}", assignmentId);
        var result = await _mediator.Send(
            new AcceptAssignmentCommand
            {
                ComplaintId = id,
                AssignmentId = assignmentId,
                StaffUserId = CurrentUserId,
            },
            ct
        );
        return Ok(result);
    }

    /// <summary>Staff views their own assignment history, across all complaints. GET api/staff/assignments</summary>
    [HttpGet("/api/staff/assignments")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetMyAssignmentHistory(
        [FromQuery] int page = PaginationConstants.DefaultPageNumber,
        [FromQuery] int limit = PaginationConstants.DefaultPageSize,
        CancellationToken ct = default
    )
    {
        _logger.LogInformation("GetMyAssignmentHistory invoked for user {UserId}", CurrentUserId);
        var result = await _mediator.Send(
            new GetStaffAssignmentHistoryQuery
            {
                CurrentUserId = CurrentUserId,
                Page = page,
                Limit = limit,
            },
            ct
        );
        return Ok(result);
    }

    /// <summary>Staff denies their assignment. PATCH api/complaints/{id}/assignments/{assignmentId}/deny</summary>
    [HttpPatch("{assignmentId:guid}/deny")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_MANAGE)]
    public async Task<IActionResult> DenyAssignment(
        Guid id,
        Guid assignmentId,
        [FromBody] DenyAssignmentRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation("DenyAssignment invoked for {AssignmentId}", assignmentId);
        var result = await _mediator.Send(
            new DenyAssignmentCommand
            {
                ComplaintId = id,
                AssignmentId = assignmentId,
                StaffUserId = CurrentUserId,
                Request = request,
            },
            ct
        );
        return Ok(result);
    }

    /// <summary>Admin reassigns a complaint. PATCH api/complaints/{id}/assignments/{assignmentId}/reassign</summary>
    [HttpPatch("{assignmentId:guid}/reassign")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_ASSIGN)]
    public async Task<IActionResult> ReassignComplaint(
        Guid id,
        Guid assignmentId,
        [FromBody] AssignComplaintRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation("ReassignComplaint invoked for {ComplaintId}", id);
        var result = await _mediator.Send(
            new ReassignComplaintCommand
            {
                ComplaintId = id,
                AssignmentId = assignmentId,
                AssignedBy = CurrentUserId,
                Request = request,
            },
            ct
        );
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
