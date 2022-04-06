using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using TaskManager.Areas.Identity.Data;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class TicketIsSubmitterAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Ticket>
{
    UserManager<ApplicationUser> _userManager;

    public TicketIsSubmitterAuthorizationHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    OperationAuthorizationRequirement requirement,
                                                    Ticket resource)
    {
        if (context.User == null || resource == null)
        {
            return Task.CompletedTask;
        }

        // If not asking for CRUD permission, return
        if (requirement.Name != Constants.CreateOperationName &&
            requirement.Name != Constants.ReadOperationName &&
            requirement.Name != Constants.UpdateOperationName &&
            requirement.Name != Constants.DeleteOperationName)
        {
            return Task.CompletedTask;
        }

        if (resource.SubmittedById == _userManager.GetUserId(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

