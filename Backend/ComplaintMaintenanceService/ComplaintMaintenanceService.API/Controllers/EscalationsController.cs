using System.Security.Claims;
using ComplaintMaintenanceService.API.Helpers;
using ComplaintMaintenanceService.Application.Features.Escalation.Commands;
using ComplaintMaintenanceService.Application.Features.Escalation.DTOs;
using ComplaintMaintenanceService.Application.Features.Escalation.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using static ComplaintMaintenanceService.API.Helpers.ClaimsHelper;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Route("api/escalations")]
[Authorize]
public class EscalationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EscalationsController> _logger;

    public EscalationsController(IMediator mediator, ILogger<EscalationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Resident re-escalates their own unresolved complaint. POST api/escalations/{complaintId}</summary>
    [HttpPost("{complaintId:guid}")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_ESCALATE)]
    public async Task<IActionResult> ReEscalate(
        Guid complaintId,
        [FromBody] ReEscalateRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            "ReEscalate action invoked for complaint {ComplaintId}",
            complaintId
        );

        var result = await _mediator.Send(
            new ReEscalateComplaintCommand
            {
                ComplaintId = complaintId,
                ResidentId = CurrentUserId,
                AdminId = GetGuidClaimOrEmpty(User, "admin_id"),
                EscalationReason = request.EscalationReason,
            },
            ct
        );

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Admin or Staff views escalation details. GET api/escalations/{complaintId}</summary>
    [HttpGet("{complaintId:guid}")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetEscalation(Guid complaintId, CancellationToken ct)
    {
        _logger.LogInformation(
            "GetEscalation action invoked for complaint {ComplaintId}",
            complaintId
        );

        var result = await _mediator.Send(new GetEscalationQuery { ComplaintId = complaintId }, ct);

        return Ok(result);
    }

    /// <summary>Admin or staff updates escalation resolution status. PUT api/escalations/{complaintId}</summary>
    [HttpPut("{complaintId:guid}")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> UpdateEscalation(
        Guid complaintId,
        [FromBody] UpdateEscalationRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation(
            "UpdateEscalation action invoked for complaint {ComplaintId}",
            complaintId
        );

        var result = await _mediator.Send(
            new UpdateEscalationCommand
            {
                ComplaintId = complaintId,
                UpdatedBy = CurrentUserId,
                Request = request,
            },
            ct
        );

        return Ok(result);
    }
}
