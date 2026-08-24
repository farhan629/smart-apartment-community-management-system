using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Shared.SharedLibrary.Extensions;
using Shared.SharedLibrary.Services;

namespace Shared.SharedLibrary.Attributes;

/// <summary>
/// Authorizes access based on user permissions.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PermissionAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _permissionCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAuthorizeAttribute"/> class.
    /// </summary>
    public PermissionAuthorizeAttribute(string permissionCode)
    {
        _permissionCode = permissionCode;
    }

    /// <summary>
    /// Handles authorization by checking if the current user has the required permission.
    /// </summary>
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var userId = context.HttpContext.User.GetCurrentUserId();

        if (userId == Guid.Empty)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var permissionService =
            context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();

        var hasPermission = await permissionService.HasPermissionAsync(userId, _permissionCode);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
