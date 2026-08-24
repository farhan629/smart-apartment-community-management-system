using IdentityService.Application.Features.Users.Commands;
using IdentityService.Application.Features.Users.DTOs;
using IdentityService.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Controller for managing users, including retrieval, updating, and soft-deletion.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance to send queries and commands.</param>
        /// <param name="logger">The logger instance for logging events.</param>
        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Returns a paginated list of all users. Admin only.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionConst.USER_MANAGE)]
        [ProducesResponseType(typeof(PaginatedResponseDto<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? name = null,
            [FromQuery] Guid? roleId = null
        )
        {
            var result = await _mediator.Send(
                new GetAllUsersQuery
                {
                    Page = page,
                    Limit = limit,
                    Name = name,
                    RoleId = roleId,
                }
            );
            return Ok(result);
        }

        /// <summary>
        /// Returns a single user by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [PermissionAuthorize(PermissionConst.USER_VIEW)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing user's profile details.
        /// </summary>
        [HttpPut("{id:guid}")]
        [PermissionAuthorize(PermissionConst.USER_VIEW)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto request)
        {
            var result = await _mediator.Send(new UpdateUserCommand { Id = id, Request = request });
            return Ok(result);
        }

        /// <summary>
        /// Soft-deletes a user. Admin only.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [PermissionAuthorize(PermissionConst.USER_VIEW)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteUserCommand { Id = id });
            return NoContent();
        }
    }
}
