namespace IdentityService.Application.Features.Approvals.DTOs
{
    /// <summary>
    /// Represents a flat-occupancy approval record as seen by an admin.
    /// Backed by FlatOccupancy (IsApproved = false → pending, true → approved).
    /// </summary>
    public class ApprovalDetailDto
    {
        /// <summary>Gets or sets the unique identifier of the approval record.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the unique identifier of the user requesting approval.</summary>
        public Guid UserId { get; set; }

        /// <summary>Gets or sets the name of the user requesting approval.</summary>
        public string? UserName { get; set; }

        /// <summary>Gets or sets the email address of the user.</summary>
        public string? Email { get; set; }

        /// <summary>Gets or sets the unique identifier of the associated flat.</summary>
        public Guid FlatId { get; set; }

        /// <summary>Gets or sets the number of the flat.</summary>
        public string? FlatNumber { get; set; }

        /// <summary>Gets or sets the block name of the flat.</summary>
        public string? Block { get; set; }

        /// <summary>Gets or sets the resident type (e.g., Owner, Tenant).</summary>
        public string? ResidentType { get; set; }

        /// <summary>Gets or sets a value indicating whether the request has been approved.</summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Human-readable status derived from IsApproved ("pending" or "approved").
        /// </summary>
        public string Status { get; set; } = "pending";

        /// <summary>Gets or sets any remarks provided by the administrator.</summary>
        public string? Remarks { get; set; }

        /// <summary>Gets or sets the timestamp when the request was created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Gets or sets the timestamp when the request was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Request body for PUT /api/approval/{id}.
    /// </summary>
    public class UpdateApprovalRequestDto
    {
        /// <summary>true = approve, false = reject.</summary>
        public bool IsApproved { get; set; }

        /// <summary>Optional admin remarks (reason for rejection, notes, etc.).</summary>
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Response body for PUT /api/approval/{id}.
    /// </summary>
    public class UpdateApprovalResponseDto
    {
        /// <summary>Gets or sets a message describing the outcome of the update operation.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Gets or sets the details of the updated approval record.</summary>
        public ApprovalDetailDto Approval { get; set; } = null!;
    }

    /// <summary>
    /// Paginated approvals list response.
    /// </summary>
    public class PaginatedApprovalResponseDto
    {
        /// <summary>Gets or sets the current page number.</summary>
        public int Page { get; set; }

        /// <summary>Gets or sets the page limit/size.</summary>
        public int Limit { get; set; }

        /// <summary>Gets or sets the total number of records matching the query.</summary>
        public int Total { get; set; }

        /// <summary>Gets the total number of pages based on Total and Limit.</summary>
        public int TotalPages => (int)Math.Ceiling((double)Total / Limit);

        /// <summary>Gets or sets the collection of approval items for the current page.</summary>
        public IEnumerable<ApprovalDetailDto> Items { get; set; } = [];
    }
}
