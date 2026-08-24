using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visits.Commands;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ResidentVisitorService.API.Controllers;

[ApiController]
[Route("api/visits")]
[Authorize]
public class VisitCheckController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisitCheckController"/> class.
    /// </summary>
    public VisitCheckController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Checks in a visitor using the provided visit token.
    /// </summary>
    [HttpPost("checkin")]
    [PermissionAuthorize(PermissionConst.VISIT_CHECKIN)]
    public async Task<IActionResult> CheckIn(
        [FromForm] string token,
        CancellationToken cancellationToken
    )
    {
        await _mediator.Send(new CheckInVisitByTokenCommand { Token = token }, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Checks out a visitor using the provided visit token.
    /// </summary>
    [HttpPost("checkout")]
    [PermissionAuthorize(PermissionConst.VISIT_CHECKIN)]
    public async Task<IActionResult> CheckOut(
        [FromForm] string token,
        CancellationToken cancellationToken
    )
    {
        await _mediator.Send(new CheckOutVisitByTokenCommand { Token = token }, cancellationToken);
        return Ok();
    }
}
