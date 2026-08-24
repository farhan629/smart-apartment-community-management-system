using IdentityService.Application.Features.Approvals.Commands;
using IdentityService.Application.Features.Approvals.DTOs;
using IdentityService.Application.Features.Approvals.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Admin endpoints for reviewing and approving/rejecting pending flat-occupancy
    /// (user registration) requests.
    /// </summary>
    [ApiController]
    [Route("api/approval")]
    [PermissionAuthorize(PermissionConst.APPROVAL_VIEW)]
    public class ApprovalController : ControllerBase
    {
        private readonly ILogger<ApprovalController> _logger;
        private readonly IMediator _mediator;

        public ApprovalController(ILogger<ApprovalController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all approvals (optionally filtered by status/userId), or a single approval by id.
        /// Use status=pending to see approved-pending occupancy requests still awaiting action,
        /// and status=approved to see ones already approved.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionConst.APPROVAL_VIEW)]
        [ProducesResponseType(typeof(PaginatedApprovalResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApprovalDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(
            [FromQuery] Guid? id,
            [FromQuery] Guid? userId,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
        )
        {
            var result = await _mediator.Send(
                new GetApprovalsQuery
                {
                    Id = id,
                    UserId = userId,
                    Status = status,
                    Page = page,
                    Limit = limit,
                }
            );

            return Ok(result);
        }

        /// <summary>
        /// Approves or rejects a pending user registration / flat-occupancy request.
        /// </summary>
        [HttpPut("{id:guid}")]
        [PermissionAuthorize(PermissionConst.APPROVAL_MANAGE)]
        [ProducesResponseType(typeof(UpdateApprovalResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateApproval(
            Guid id,
            [FromBody] UpdateApprovalRequestDto request
        )
        {
            var result = await _mediator.Send(
                new UpdateApprovalCommand { Id = id, Request = request }
            );

            return Ok(result);
        }
    }
}
