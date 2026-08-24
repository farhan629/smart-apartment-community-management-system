using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.VisitorTypes.Queries;
using ResidentVisitorService.Application.Features.VisitQrToken.DTOs;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ResidentVisitorService.API.Controllers;

/// <summary>
/// Provides lookup endpoints for visitor types and purpose types.
/// </summary>
[ApiController]
[Route("api")]
[Authorize]
public class VisitorTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VisitorTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns all active visitor types.
    /// </summary>
    [HttpGet("visitor-types")]
    [PermissionAuthorize(PermissionConst.VISIT_VIEW)]
    public async Task<IActionResult> GetVisitorTypes(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVisitorTypesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns all active purpose types.
    /// </summary>
    [HttpGet("purpose-types")]
    [PermissionAuthorize(PermissionConst.VISIT_VIEW)]
    public async Task<IActionResult> GetPurposeTypes(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPurposeTypesQuery(), cancellationToken);
        return Ok(result);
    }
}
