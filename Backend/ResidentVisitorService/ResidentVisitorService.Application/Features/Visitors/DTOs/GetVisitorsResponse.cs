using Shared.SharedLibrary.DTO.Common;

namespace ResidentVisitorService.Application.Features.Visitors.DTOs;

/// <summary>Response payload for a paginated list of visitors.</summary>
public class GetVisitorsResponse
{
    /// <summary>The visitors for the current page.</summary>
    public List<VisitorResponseDto> Items { get; set; } = [];

    /// <summary>Pagination metadata (page, page size, total count).</summary>
    public PaginationDto Pagination { get; set; } = new();
}
