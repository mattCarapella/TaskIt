﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using TaskManager.Core;
using TaskManager.Models;

namespace TaskManager.Authorization;

public class TNoteAdminAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, TNote>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   OperationAuthorizationRequirement requirement,
                                                   TNote resource)
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
