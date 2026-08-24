using System.Security.Claims;
using ComplaintMaintenanceService.Application.Features.Comments.Commands;
using ComplaintMaintenanceService.Application.Features.Comments.DTOs;
using ComplaintMaintenanceService.Application.Features.Comments.Queries;
using ComplaintMaintenanceService.Application.Features.ProgressLog.DTOs;
using ComplaintMaintenanceService.Application.Features.ProgressLog.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(IMediator mediator, ILogger<CommentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Resident adds a follow-up comment on their own complaint. POST api/complaints/{complaintId}/comments</summary>
    [HttpPost("api/complaints/{Id:guid}/comments")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_COMMENT)]
    public async Task<IActionResult> AddComment(
        Guid complaintId,
        [FromBody] CreateCommentRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation("AddComment invoked for complaint {ComplaintId}", complaintId);
        var result = await _mediator.Send(
            new CreateCommentCommand
            {
                ComplaintId = complaintId,
                CommentedBy = CurrentUserId,
                Request = request,
            },
            ct
        );
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Admin and Staff view all comments. Resident views own complaint comments. GET api/complaints/{complaintId}/comments</summary>
    [HttpGet("api/complaints/{Id:guid}/comments")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetComments(Guid complaintId, CancellationToken ct)
    {
        _logger.LogInformation("GetComments invoked for complaint {ComplaintId}", complaintId);
        var result = await _mediator.Send(new GetCommentsQuery { ComplaintId = complaintId }, ct);
        return Ok(result);
    }

    /// <summary>Admin views all comments by a specific staff member. GET api/staff/{staffId}/comments</summary>
    [HttpGet("api/staff/{Id:guid}/comments")]
    [PermissionAuthorize(PermissionConst.STAFF_VIEW)]
    public async Task<IActionResult> GetStaffComments(Guid staffId, CancellationToken ct)
    {
        _logger.LogInformation("GetStaffComments invoked for staff {StaffId}", staffId);
        var result = await _mediator.Send(new GetStaffCommentsQuery { StaffId = staffId }, ct);
        return Ok(result);
    }

    /// <summary>Admin, Staff, Resident view progress log of a complaint. GET api/complaints/{complaintId}/progress-log</summary>
    [HttpGet("api/complaints/{Id:guid}/progress-log")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetProgressLog(Guid complaintId, CancellationToken ct)
    {
        _logger.LogInformation("GetProgressLog invoked for complaint {ComplaintId}", complaintId);
        var result = await _mediator.Send(
            new GetProgressLogQuery { ComplaintId = complaintId },
            ct
        );
        return Ok(result);
    }
}
