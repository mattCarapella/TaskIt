using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TaskManager.Core;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class TNoteManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, TNote> 
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   OperationAuthorizationRequirement requirement,
                                                   TNote resource)
    {
        if (context.User is null)
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

        if (context.User.IsInRole(Constants.Roles.Manager))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
