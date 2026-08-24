using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Amenities.Commands;
using AmenityBookingService.Application.Features.Amenities.DTO;
using AmenityBookingService.Application.Features.Amenities.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.API.Controllers;

/// <summary>
/// Controller for managing amenity CRUD operations and image uploads.
/// </summary>
[ApiController]
[Route("api/amenity")]
[Authorize]
public class AmenityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmenityController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR request mediator.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public AmenityController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a paginated list of amenities with optional search and slot type filter.
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The page size (default: 10).</param>
    /// <param name="searchName">Optional name search filter.</param>
    /// <param name="slotType">Optional slot type code filter.</param>
    /// <returns>A paginated list of amenity response DTOs.</returns>
    [HttpGet]
    [PermissionAuthorize(PermissionConst.AMENITY_VIEW)]
    public async Task<ActionResult<AmenityListResponseDto>> GetAmenities(
        [FromQuery] int pageNumber = PaginationConstants.DefaultPageNumber,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        [FromQuery] string? searchName = null,
        [FromQuery] string? slotType = null
    )
    {
        var query = new GetAmenitiesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchName = searchName,
            SlotType = slotType,
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single amenity by its unique identifier.
    /// </summary>
    /// <param name="id">The amenity ID.</param>
    /// <returns>The amenity response DTO.</returns>
    [HttpGet("{id}")]
    [PermissionAuthorize(PermissionConst.AMENITY_VIEW)]
    public async Task<ActionResult<AmenityResponseDto>> GetAmenityById(Guid id)
    {
        var query = new GetAmenityByIdQuery { AmenityId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new amenity.
    /// </summary>
    /// <param name="dto">The create amenity request DTO.</param>
    /// <returns>The ID of the newly created amenity.</returns>
    [HttpPost]
    [PermissionAuthorize(PermissionConst.AMENITY_MANAGE)]
    public async Task<ActionResult<IdResponseDto>> CreateAmenity(
        [FromBody] CreateAmenityRequestDto dto
    )
    {
        var command = _mapper.Map<CreateAmenityCommand>(dto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAmenityById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing amenity.
    /// </summary>
    /// <param name="id">The amenity ID.</param>
    /// <param name="dto">The update amenity request DTO.</param>
    /// <returns>A message response indicating success.</returns>
    [HttpPut("{id}")]
    [PermissionAuthorize(PermissionConst.AMENITY_MANAGE)]
    public async Task<ActionResult<MessageResponseDto>> UpdateAmenity(
        Guid id,
        [FromBody] UpdateAmenityRequestDto dto
    )
    {
        var command = _mapper.Map<UpdateAmenityCommand>(dto);
        command.AmenityId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Uploads an amenity image file. Supported types: .jpg, .jpeg, .png, .svg. Max size: 10 MB.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>The accessible URL of the uploaded image.</returns>
    [HttpPost("upload")]
    [RequestSizeLimit(Application.Constants.RefTermCodes.MaxImageSizeInBytes)]
    [AllowAnonymous]
    public async Task<ActionResult<UploadImageResponseDto>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(Shared.SharedLibrary.Constants.ErrorMessages.NoFileProvided);

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var command = new UploadAmenityImageCommand
        {
            FileName = file.FileName,
            FileContent = ms.ToArray(),
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Soft-deletes an amenity by marking it as inactive. Fails if the amenity has active slots or booking history.
    /// </summary>
    /// <param name="id">The amenity ID to delete.</param>
    /// <returns>A message response indicating success.</returns>
    [HttpDelete("{id}")]
    [PermissionAuthorize(PermissionConst.AMENITY_MANAGE)]
    public async Task<ActionResult<MessageResponseDto>> DeleteAmenity(Guid id)
    {
        var command = new DeleteAmenityCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
