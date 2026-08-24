using System.Security.Claims;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.Commands;
using ComplaintMaintenanceService.Application.Features.Complaints.DTOs;
using ComplaintMaintenanceService.Application.Features.Complaints.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Extensions;
using static ComplaintMaintenanceService.API.Helpers.ResidentScopeHelper;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Route("api/complaints")]
[Authorize]
public class ComplaintsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ComplaintsController> _logger;

    public ComplaintsController(IMediator mediator, ILogger<ComplaintsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [PermissionAuthorize(PermissionConst.COMPLAINT_SUBMIT)]
    public async Task<IActionResult> CreateComplaint(
        [FromBody] CreateComplaintRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation("CreateComplaint action invoked");
        var result = await _mediator.Send(new CreateComplaintCommand { Request = request }, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Admin and Staff view all complaints. Resident views own. GET api/complaints</summary>
    [HttpGet]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetComplaints(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] Guid? categoryId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = PaginationConstants.DefaultPageNumber,
        [FromQuery] int limit = PaginationConstants.DefaultPageSize,
        CancellationToken ct = default
    )
    {
        _logger.LogInformation("GetComplaints action invoked");

        var residentScopeUserId = ResolveResidentScopeUserId(User);

        var query = new GetComplaintsQuery
        {
            Status = status,
            Priority = priority,
            CategoryId = categoryId,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            Limit = limit,
            IsResident = User.GetCurrentRoleId() == RoleIds.Resident,
            CurrentUserId = User.GetCurrentUserId(),
            CurrentRoleId = User.GetCurrentRoleId(),
        };

        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>Admin, Staff, Resident view complaint detail. GET api/complaints/{complaintId}</summary>
    [HttpGet(ComplaintConstants.Routes.ComplaintId)]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetComplaintById(Guid complaintId, CancellationToken ct)
    {
        _logger.LogInformation("GetComplaintById action invoked for {ComplaintId}", complaintId);

        var query = new GetComplaintByIdQuery
        {
            ComplaintId = complaintId,
            IsResident = User.IsInRole(ComplaintConstants.Roles.Resident),
        };

        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>Staff or Admin updates complaint status. PATCH api/complaints/{complaintId}/status</summary>
    [HttpPatch(ComplaintConstants.Routes.Status)]
    [PermissionAuthorize(PermissionConst.COMPLAINT_MANAGE)]
    public async Task<IActionResult> UpdateComplaintStatus(
        Guid complaintId,
        [FromBody] ComplaintStatusUpdateRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            "UpdateComplaintStatus action invoked for {ComplaintId}",
            complaintId
        );
        var result = await _mediator.Send(
            new UpdateComplaintStatusCommand { ComplaintId = complaintId, Request = request },
            ct
        );
        return Ok(result);
    }

    /// <summary>Resident cancels their own complaint. PATCH api/complaints/{complaintId}/cancel</summary>
    [HttpPatch(ComplaintConstants.Routes.Cancel)]
    [PermissionAuthorize(PermissionConst.COMPLAINT_CANCEL)]
    public async Task<IActionResult> CancelComplaint(
        Guid complaintId,
        [FromBody] ComplaintCancelRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation("CancelComplaint action invoked for {ComplaintId}", complaintId);
        var result = await _mediator.Send(
            new CancelComplaintCommand { ComplaintId = complaintId, Request = request },
            ct
        );
        return Ok(result);
    }
}
