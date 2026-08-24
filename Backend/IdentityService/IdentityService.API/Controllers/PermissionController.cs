using IdentityService.Application.Features.Permissions.Commands;
using IdentityService.Application.Features.Permissions.DTOs;
using IdentityService.Application.Features.Permissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace IdentityService.API.Controllers;

/// <summary>
/// Controller for managing user permissions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public PermissionController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets the current authenticated user's permissions
    /// </summary>
    /// <returns>User permissions</returns>
    [HttpGet("me")]
    public async Task<ActionResult<UserPermissionsDto>> GetMyPermissions()
    {
        var userId = _currentUserService.UserId;

        if (userId == Guid.Empty)
            throw new UnauthorizedException("User not authenticated");

        var query = new GetUserPermissionsQuery { UserId = userId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Gets permissions for a specific user (admin only)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User permissions with policies</returns>
    [HttpGet("{userId}")]
    [PermissionAuthorize(PermissionConst.USER_MANAGE)]
    public async Task<ActionResult<UserPermissionsResponseDto>> GetUserPermissions(Guid userId)
    {
        var query = new GetUserPermissionsWithPoliciesQuery { UserId = userId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Assigns permissions to a user (admin only)
    /// </summary>
    /// <param name="dto">Permission assignment DTO</param>
    /// <returns>Success message</returns>
    [HttpPut("assign")]
    [PermissionAuthorize(PermissionConst.USER_MANAGE)]
    public async Task<IActionResult> AssignUserPermissions(
        [FromBody] AssignUserPermissionsRequestDto dto
    )
    {
        var command = new AssignUserPermissionsCommand
        {
            UserId = dto.UserId,
            Permissions = dto.Permissions,
        };

        var result = await _mediator.Send(command);

        return Ok(new { message = result });
    }
}
