namespace IdentityService.Application.Features.Flats.DTOs
{
    /// <summary>
    /// Represents the details of a flat returned in API responses.
    /// </summary>
    public class FlatResponseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the flat.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the flat number.
        /// </summary>
        public string Number { get; set; } = null!;

        /// <summary>
        /// Gets or sets the block in which the flat is located.
        /// </summary>
        public string Block { get; set; } = null!;

        /// <summary>
        /// Gets or sets the floor number of the flat.
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the flat is available for occupancy.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the flat record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}