﻿using System.Security.Claims;
using Backend.Data.Entities.Auth;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Backend.Services.Auth;

public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement, IUserOwnedResource>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOwnerRequirement requirement, IUserOwnedResource resource)
    {
        if (context.User.IsInRole(ApplicationUserRoles.Admin) || context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) == resource.OwnerId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public record ResourceOwnerRequirement : IAuthorizationRequirement;

