using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TaskManager.Core;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class PNoteManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, PNote>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   OperationAuthorizationRequirement requirement,
                                                   PNote resource)
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

        // Admin can do anything
        if (context.User.IsInRole(Constants.Roles.Manager))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
