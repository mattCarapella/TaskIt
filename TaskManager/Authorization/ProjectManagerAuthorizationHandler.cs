using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class ProjectManagerAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Project>
{
    UserManager<ApplicationUser> _userManager;

    public ProjectManagerAuthorizationHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    OperationAuthorizationRequirement requirement,
                                                    Project resource)
    {
        if (context.User is null || resource is null)
        {
            return Task.CompletedTask;
        }

        // If not asking for CRUD permission, return
        if (requirement.Name != AuthConstants.CreateOperationName &&
            requirement.Name != AuthConstants.ReadOperationName &&
            requirement.Name != AuthConstants.UpdateOperationName)
        {
            return Task.CompletedTask;
        }

        if (resource.CreatedByUserId == _userManager.GetUserId(context.User) &&
            context.User.IsInRole(Constants.Roles.Manager))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

