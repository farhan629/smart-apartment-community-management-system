using System.Security.Claims;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Common.Pagination;
using ComplaintMaintenanceService.Application.Features.Staff.DTOs;
using ComplaintMaintenanceService.Application.Features.Staff.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StaffController> _logger;

    public StaffController(IMediator mediator, ILogger<StaffController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Admin or Staff views staff list. GET api/staff</summary>
    [HttpGet]
    [PermissionAuthorize(PermissionConst.STAFF_VIEW)]
    public async Task<IActionResult> GetStaff(
        [FromQuery] int page = PaginationConstants.DefaultPageNumber,
        [FromQuery] int limit = PaginationConstants.DefaultPageSize,
        CancellationToken ct = default
    )
    {
        _logger.LogInformation("GetStaff action invoked");

        var result = await _mediator.Send(new GetStaffQuery { Page = page, Limit = limit }, ct);

        return Ok(result);
    }

    /// <summary>Admin or Staff views a staff profile. GET api/staff/{staffId}</summary>
    [HttpGet(StaffConstants.Routes.StaffId)]
    [PermissionAuthorize(PermissionConst.STAFF_VIEW)]
    public async Task<IActionResult> GetStaffById(Guid staffId, CancellationToken ct)
    {
        _logger.LogInformation("GetStaffById action invoked for {StaffId}", staffId);
        var result = await _mediator.Send(new GetStaffByIdQuery { StaffId = staffId }, ct);
        return Ok(result);
    }
}
