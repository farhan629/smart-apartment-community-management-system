using System.Security.Claims;
using ComplaintMaintenanceService.Application.Features.BackgroundJobs.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize]
[PermissionAuthorize(PermissionConst.JOB_TRIGGER)]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IMediator mediator, ILogger<JobsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Admin manually triggers the escalation check background job. POST api/jobs/escalation-check</summary>
    [HttpPost("escalation-check")]
    public async Task<IActionResult> RunEscalationCheck(CancellationToken ct)
    {
        _logger.LogInformation("RunEscalationCheck invoked by {UserId}", CurrentUserId);

        var result = await _mediator.Send(
            new RunEscalationCheckCommand { TriggeredBy = CurrentUserId },
            ct
        );

        return Ok(result);
    }
}
