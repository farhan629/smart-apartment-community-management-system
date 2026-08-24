using Shared.SharedLibrary.DTO;

namespace IdentityService.Domain.Entities;

/// <summary>
/// Represents a user in the Identity Service.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique email used for login and communication.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    public string PhoneNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the profile photo URL of the user.
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the role associated with the user.
    /// </summary>
    public virtual RefTerm? Role { get; set; }

    /// <summary>
    /// Gets or sets the password security details associated with the user.
    /// </summary>
    public virtual UserPasswordSecurity? PasswordSecurity { get; set; }

    /// <summary>
    /// Gets or sets the collection of user policies associated with the user.
    /// </summary>
    public virtual ICollection<UserPolicy>? UserPolicies { get; set; }

    /// <summary>
    /// Gets or sets the collection of flat occupancies associated with the user.
    /// </summary>
    public virtual ICollection<FlatOccupancy>? FlatOccupancies { get; set; }
}
