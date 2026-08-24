using Shared.SharedLibrary.DTO.Common;

namespace ResidentVisitorService.Application.Features.Visits.DTOs;

/// <summary>Response payload for a paginated list of visits.</summary>
public class GetVisitsResponse
{
    /// <summary>The visits for the current page.</summary>
    public List<VisitResponseDto> Items { get; set; } = [];

    /// <summary>Pagination metadata (page, page size, total count).</summary>
    public PaginationDto Pagination { get; set; } = new();
}
