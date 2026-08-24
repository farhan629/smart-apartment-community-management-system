using FluentValidation;

namespace IdentityService.Application.Features.Permissions.Commands;

public class AssignUserPermissionsCommandValidator : AbstractValidator<AssignUserPermissionsCommand>
{
    public AssignUserPermissionsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Permissions)
            .NotEmpty()
            .WithMessage("At least one permission entry is required.");

        RuleForEach(x => x.Permissions)
            .ChildRules(p =>
            {
                p.RuleFor(x => x.PermissionCode)
                    .NotEmpty()
                    .WithMessage("PermissionCode is required for each entry.");
            });

        RuleFor(x => x.Permissions)
            .Must(p => p.Select(e => e.PermissionCode).Distinct().Count() == p.Count)
            .WithMessage("Duplicate PermissionCode entries are not allowed.");
    }
}
