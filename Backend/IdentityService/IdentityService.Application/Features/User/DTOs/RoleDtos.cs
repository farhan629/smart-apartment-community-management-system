namespace IdentityService.Application.Features.Roles.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a user role.
    /// </summary>
    public class RoleDto
    {
        /// <summary>Gets or sets the unique identifier of the role.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the role term value (e.g. "Admin", "Owner").</summary>
        public string TermValue { get; set; } = string.Empty;

        /// <summary>Gets or sets the description of the role.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the category of the role ("Occupant" or "Management").</summary>
        public string Category { get; set; } = string.Empty;
    }

}
