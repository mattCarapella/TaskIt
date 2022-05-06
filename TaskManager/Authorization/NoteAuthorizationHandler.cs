using Microsoft.AspNetCore.Authorization;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class NoteAuthorizationHandler : AuthorizationHandler<SameUserRequirement, PNote>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, PNote resource)
    {
        if (context.User.Identity?.Name == resource.ApplicationUser.UserName)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

public class SameUserRequirement : IAuthorizationRequirement { }
