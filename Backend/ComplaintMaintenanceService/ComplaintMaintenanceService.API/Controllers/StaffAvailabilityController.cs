using ComplaintMaintenanceService.API.Helpers;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.Commands;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using static ComplaintMaintenanceService.API.Helpers.DateParsingHelper;

namespace ComplaintMaintenanceService.API.Controllers;

[ApiController]
[Route("api/staff/availability")]
[Authorize]
public class StaffAvailabilityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StaffAvailabilityController> _logger;

    public StaffAvailabilityController(
        IMediator mediator,
        ILogger<StaffAvailabilityController> logger
    )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [PermissionAuthorize(PermissionConst.STAFF_AVAILABILITY_VIEW)]
    public async Task<IActionResult> GetAvailability(
        [FromQuery] Guid? staffId,
        [FromQuery] string? date,
        [FromQuery] Guid? categoryId,
        [FromQuery] bool? isBooked,
        [FromQuery] string? fromDate,
        [FromQuery] string? toDate,
        [FromQuery] string? startTime,
        [FromQuery] string? endTime,
        CancellationToken ct
    )
    {
        _logger.LogInformation(StaffAvailabilityConstants.Messages.SlotsFetched);

        var result = await _mediator.Send(
            new GetStaffAvailabilityQuery
            {
                StaffId = staffId,
                Date = ParseDateUtc(date),
                CategoryId = categoryId,
                IsBooked = isBooked,
                FromDate = ParseDateUtc(fromDate),
                ToDate = ParseDateUtc(toDate),
                StartTime = TimeSpan.TryParse(startTime, out var start) ? start : null,
                EndTime = TimeSpan.TryParse(endTime, out var end) ? end : null,
            },
            ct
        );

        return Ok(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionConst.STAFF_AVAILABILITY_MANAGE)]
    public async Task<IActionResult> CreateAvailability(
        [FromQuery] Guid staffId,
        [FromBody] CreateAvailabilityRequestDto request,
        CancellationToken ct
    )
    {
        _logger.LogInformation(StaffAvailabilityConstants.Messages.SlotsCreated);

        var result = await _mediator.Send(
            new CreateStaffAvailabilityCommand { StaffId = staffId, Request = request },
            ct
        );

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet(StaffAvailabilityConstants.Routes.SlotId)]
    [PermissionAuthorize(PermissionConst.STAFF_AVAILABILITY_VIEW)]
    public async Task<IActionResult> GetAvailabilityById(
        Guid slotId,
        [FromQuery] Guid staffId,
        CancellationToken ct
    )
    {
        _logger.LogInformation(StaffAvailabilityConstants.Messages.SlotFetched);

        var result = await _mediator.Send(
            new GetStaffAvailabilityByIdQuery { SlotId = slotId, StaffId = staffId },
            ct
        );

        return Ok(result);
    }

    [HttpDelete(StaffAvailabilityConstants.Routes.SlotId)]
    [PermissionAuthorize(PermissionConst.STAFF_AVAILABILITY_MANAGE)]
    public async Task<IActionResult> DeleteAvailability(
        Guid slotId,
        [FromQuery] Guid staffId,
        CancellationToken ct
    )
    {
        _logger.LogInformation(StaffAvailabilityConstants.Messages.SlotDeleted);

        await _mediator.Send(
            new DeleteStaffAvailabilityCommand { SlotId = slotId, StaffId = staffId },
            ct
        );

        return Ok();
    }
}
