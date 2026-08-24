using System.Security.Claims;
using ComplaintMaintenanceService.API.Helpers;
using ComplaintMaintenanceService.Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using static ComplaintMaintenanceService.API.Helpers.DateParsingHelper;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IMediator mediator, ILogger<ReportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Admin views complaint statistics report. GET api/reports</summary>
    [HttpGet]
    [PermissionAuthorize(PermissionConst.REPORT_VIEW)]
    public async Task<IActionResult> GetReport(
        [FromQuery] string? fromDate,
        [FromQuery] string? toDate,
        CancellationToken ct
    )
    {
        _logger.LogInformation("GetReport invoked");

        var result = await _mediator.Send(
            new GetReportQuery { FromDate = ParseDateUtc(fromDate), ToDate = ParseDateUtc(toDate) },
            ct
        );

        return Ok(result);
    }
}
