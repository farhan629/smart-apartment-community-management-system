using FluentValidation;
using IdentityService.Application.Features.Permissions.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Permissions.Commands;

/// <summary>
/// Command to assign permissions to a user
/// </summary>
public class AssignUserPermissionsCommand : IRequest<string>
{
    public Guid UserId { get; set; }
    public List<UserPermissionEntryDto> Permissions { get; set; } = new();
}

/// <summary>
/// Handler for AssignUserPermissionsCommand
/// </summary>
public class AssignUserPermissionsCommandHandler
    : IRequestHandler<AssignUserPermissionsCommand, string>
{
    private readonly IUserRepository _userRepository;

    public AssignUserPermissionsCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handles the assignment of user permissions
    /// </summary>
    /// <param name="request">Command containing user ID and permissions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    public async Task<string> Handle(
        AssignUserPermissionsCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetUserWithRoleAsync(request.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException("User not found");

        var policies = request.Permissions.Select(p => new UserPolicy
        {
            UserId = request.UserId,
            PermissionCode = p.PermissionCode,
            IsAllowed = p.IsAllowed,
        });

        await _userRepository.UpsertUserPoliciesAsync(policies, cancellationToken);

        return "Updation Successful";
    }
}
