using IdentityService.Application.Features.Roles.DTOs;
using IdentityService.Application.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers
{
    /// <summary>
    /// Manages Occupant (Owner/Tenant) and Management (Admin/Staff) roles.
    /// </summary>
    [ApiController]
    [Route("api/role")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IMediator mediator, ILogger<RolesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>Returns all Occupant roles (Owner / Tenant).</summary>
        [HttpGet("occupant")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOccupantRoles() =>
            Ok(await _mediator.Send(new GetRolesQuery { Category = "Occupant" }));

        /// <summary>Returns all Management roles (Admin / Staff).</summary>
        [HttpGet("management")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetManagementRoles() =>
            Ok(await _mediator.Send(new GetRolesQuery { Category = "Management" }));

        /// <summary>Returns a single role by its ID.</summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id) =>
            Ok(await _mediator.Send(new GetRoleByIdQuery { Id = id }));
    }
}
