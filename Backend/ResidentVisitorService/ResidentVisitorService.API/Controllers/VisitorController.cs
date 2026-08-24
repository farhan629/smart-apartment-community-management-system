using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.Commands;
using ResidentVisitorService.Application.Features.Visitors.DTOs;
using ResidentVisitorService.Application.Features.Visitors.Queries;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace ResidentVisitorService.API.Controllers;

/// <summary>
/// Manages visitor records — create, list, update, soft-delete, and photo upload.
/// </summary>
[ApiController]
[Route("api/visitors")]
[Authorize]
public class VisitorController : ControllerBase
{
    private readonly IMediator _mediator;

    public VisitorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns a paginated list of visitors. Pass an <c>id</c> query param to fetch a single record.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize(PermissionConst.VISITOR_VIEW)]
    public async Task<IActionResult> GetVisitors(
        [FromQuery] Guid? id,
        [FromQuery] string? search,
        [FromQuery] string sortBy = ResidentVisitorConstants.Pagination.DefaultSortBy,
        [FromQuery] string sortOrder = ResidentVisitorConstants.Pagination.DefaultSortOrder,
        [FromQuery] int page = PaginationConstants.DefaultPageNumber,
        [FromQuery] int limit = PaginationConstants.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetVisitorsQuery
        {
            Id = id,
            Search = search,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            Limit = Math.Min(limit, PaginationConstants.MaxPageSize),
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (result.Pagination.TotalCount == 0)
        {
            return NoContent();
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new visitor record.
    /// </summary>
    [HttpPost]
    [PermissionAuthorize(PermissionConst.VISITOR_MANAGE)]
    public async Task<IActionResult> CreateVisitor(
        [FromBody] CreateVisitorRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new CreateVisitorCommand { Request = request },
            cancellationToken
        );
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Updates an existing visitor record.
    /// </summary>
    [HttpPut]
    [PermissionAuthorize(PermissionConst.VISITOR_MANAGE)]
    public async Task<IActionResult> UpdateVisitor(
        [FromQuery] Guid id,
        [FromBody] UpdateVisitorRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new UpdateVisitorCommand { Id = id, Request = request },
            cancellationToken
        );

        return Ok(result);
    }

    /// <summary>
    /// Soft-deletes (deactivates) a visitor.
    /// </summary>
    [HttpDelete]
    [PermissionAuthorize(PermissionConst.VISITOR_DELETE)]
    public async Task<IActionResult> DeleteVisitor(
        [FromQuery] Guid id,
        CancellationToken cancellationToken
    )
    {
        await _mediator.Send(new DeleteVisitorCommand { Id = id }, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Uploads or replaces the profile photo for an existing visitor.
    /// </summary>
    [HttpPost("{id}/photo")]
    [Consumes("multipart/form-data")]
    [PermissionAuthorize(PermissionConst.VISITOR_MANAGE)]
    public async Task<IActionResult> UploadVisitorPhoto(
        [FromRoute] Guid id,
        IFormFile photo,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new UploadVisitorPhotoCommand { VisitorId = id, Photo = photo },
            cancellationToken
        );

        return Ok(result);
    }
}
