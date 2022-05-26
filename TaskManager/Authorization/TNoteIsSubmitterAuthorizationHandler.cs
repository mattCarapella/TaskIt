using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using TaskManager.Areas.Identity.Data;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class TNoteIsSubmitterAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, TNote>
{
    UserManager<ApplicationUser> _userManager;

    public TNoteIsSubmitterAuthorizationHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    OperationAuthorizationRequirement requirement,
                                                    TNote resource)
    {
        if (context.User is null || resource is null)
        {
            return Task.CompletedTask;
        }

        // User can create, read, and update their own notes
        if (requirement.Name != AuthConstants.CreateOperationName &&
            requirement.Name != AuthConstants.ReadOperationName &&
            requirement.Name != AuthConstants.UpdateOperationName)
        {
            return Task.CompletedTask;
        }

        if (resource.ApplicationUserId == _userManager.GetUserId(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

