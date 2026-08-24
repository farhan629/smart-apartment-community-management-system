
using Shared.SharedLibrary;
using Shared.SharedLibrary.DTO.Common;

namespace IdentityService.Application.Features.Flats.DTOs
{
    /// <summary>
    /// Represents a paginated response containing a collection of flats
    /// along with pagination metadata.
    /// </summary>
    public class PagedFlatResponseDto
    {
        /// <summary>
        /// Gets or sets the list of flats returned for the current page.
        /// </summary>
        public List<FlatResponseDto> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets pagination information such as page number,
        /// page size, total records, and total pages.
        /// </summary>
        public PaginationDto Pagination { get; set; } = null!;
    }
}