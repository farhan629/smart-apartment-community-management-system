using ComplaintMaintenanceService.Application.Features.Lookups.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Route("api/lookups")]
[Authorize]
public class LookupsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LookupsController> _logger;

    public LookupsController(IMediator mediator, ILogger<LookupsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>All roles view complaint types. GET api/lookups/complaint-types</summary>
    [HttpGet("complaint-types")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetComplaintTypes(CancellationToken ct)
    {
        _logger.LogInformation("GetComplaintTypes action invoked");
        var result = await _mediator.Send(new GetComplaintTypesQuery(), ct);
        return Ok(result);
    }

    /// <summary>All roles view complaint statuses. GET api/lookups/complaint-statuses</summary>
    [HttpGet("complaint-statuses")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetComplaintStatuses(CancellationToken ct)
    {
        _logger.LogInformation("GetComplaintStatuses action invoked");
        var result = await _mediator.Send(new GetComplaintStatusesQuery(), ct);
        return Ok(result);
    }

    /// <summary>All roles view complaint priorities. GET api/lookups/complaint-priorities</summary>
    [HttpGet("complaint-priorities")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetComplaintPriorities(CancellationToken ct)
    {
        _logger.LogInformation("GetComplaintPriorities action invoked");
        var result = await _mediator.Send(new GetComplaintPrioritiesQuery(), ct);
        return Ok(result);
    }

    /// <summary>All roles view categories. GET api/lookups/categories</summary>
    [HttpGet("categories")]
    [PermissionAuthorize(PermissionConst.COMPLAINT_VIEW)]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        _logger.LogInformation("GetCategories action invoked");
        var result = await _mediator.Send(new GetCategoriesQuery(), ct);
        return Ok(result);
    }
}
