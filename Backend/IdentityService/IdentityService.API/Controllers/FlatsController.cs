using IdentityService.Application.Features.Flats.DTOs;
using IdentityService.Application.Features.Flats.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Api.Controllers
{
    /// <summary>
    /// Controller for managing flats
    /// </summary>
    [ApiController]
    [Route("api/flats")]
    public class FlatsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlatsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a paginated list of all flats (public access)
        /// </summary>
        /// <param name="request">Pagination request</param>
        /// <returns>Paginated flats</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedFlatResponseDto>> GetFlats(
            [FromQuery] GetFlatsRequestDto request
        )
        {
            var result = await _mediator.Send(
                new GetFlatsQuery { PageNumber = request.PageNumber, PageSize = request.PageSize }
            );
            return Ok(result);
        }

        /// <summary>
        /// Gets a specific flat by ID (requires SPECIFIC_FLATS permission)
        /// </summary>
        /// <param name="id">Flat ID</param>
        /// <returns>Flat details</returns>
        [HttpGet("{id:guid}")]
        [PermissionAuthorize(PermissionConst.SPECIFIC_FLATS)]
        public async Task<ActionResult<FlatResponseDto>> GetFlatById(Guid id)
        {
            var result = await _mediator.Send(new GetFlatByIdQuery { Id = id });
            return Ok(result);
        }
    }
}
