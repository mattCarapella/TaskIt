using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TaskManager.Core;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class TicketAdminAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Ticket>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   OperationAuthorizationRequirement requirement,
                                                   Ticket resource)
    {
        if (context.User is null)
        {
            return Task.CompletedTask;
        }

        // Admin can do anything
        if (context.User.IsInRole(Constants.Roles.Administrator))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
