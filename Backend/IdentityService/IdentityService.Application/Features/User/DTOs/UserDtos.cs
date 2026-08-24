namespace IdentityService.Application.Features.Users.DTOs
{
    /// <summary>
    /// Data Transfer Object representing user details.
    /// </summary>
    public class UserDto
    {
        /// <summary>Gets or sets the unique identifier of the user.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the username/display name of the user.</summary>
        public string? UserName { get; set; }

        /// <summary>Gets or sets the email address of the user.</summary>
        public string? Email { get; set; }

        /// <summary>Gets or sets the phone number of the user.</summary>
        public string? Phone { get; set; }

        /// <summary>Gets or sets the profile photo URL of the user.</summary>
        public string? PhotoUrl { get; set; }

        /// <summary>Gets or sets the role of the user (e.g. "Admin", "Owner").</summary>
        public string? Role { get; set; }

        public Guid? FlatId { get; set; }

        /// <summary>Gets or sets a value indicating whether the user is active.</summary>
        public bool IsActive { get; set; }

        /// <summary>Gets or sets the timestamp when the user was created.</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Request payload for updating user details.
    /// </summary>
    public class UpdateUserRequestDto
    {
        /// <summary>Gets or sets the updated username.</summary>
        public string? UserName { get; set; }

        /// <summary>Gets or sets the updated phone number.</summary>
        public string? Phone { get; set; }

        /// <summary>Gets or sets the updated profile photo URL.</summary>
        public string? PhotoUrl { get; set; }
    }

    /// <summary>
    /// Generic paginated response DTO.
    /// </summary>
    /// <typeparam name="T">The type of the item collection.</typeparam>
    public class PaginatedResponseDto<T>
    {
        /// <summary>Gets or sets the current page number.</summary>
        public int Page { get; set; }

        /// <summary>Gets or sets the page size limit.</summary>
        public int Limit { get; set; }

        /// <summary>Gets or sets the total number of items matching the query.</summary>
        public int Total { get; set; }

        /// <summary>Gets the total number of pages based on Total and Limit.</summary>
        public int TotalPages => (int)Math.Ceiling((double)Total / Limit);

        /// <summary>Gets or sets the collection of items for the current page.</summary>
        public IEnumerable<T> Items { get; set; } = [];
    }
}
