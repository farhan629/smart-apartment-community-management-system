using System.ComponentModel.DataAnnotations;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;

namespace IdentityService.Application.Features.Flats.DTOs
{
    /// <summary>
    /// Represents the request parameters used to retrieve a paginated list of flats.
    /// </summary>
    public class GetFlatsRequestDto
    {
        /// <summary>
        /// Gets or sets the page number to retrieve.
        /// The value must be greater than or equal to 1.
        /// </summary>
        [Range(
            PaginationConstants.MinPageNumber,
            int.MaxValue,
            ErrorMessage = "PageNumber must be at least {1}.")]
        public int PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;

        /// <summary>
        /// Gets or sets the number of records to return per page.
        /// The value must be within the configured minimum and maximum page size limits.
        /// </summary>
        [Range(
            PaginationConstants.MinPageSize,
            PaginationConstants.MaxPageSize,
            ErrorMessage = "PageSize must be between {1} and {2}.")]
        public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
    }
}