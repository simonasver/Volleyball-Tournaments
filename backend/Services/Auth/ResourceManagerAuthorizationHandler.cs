using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Data.Entities.Auth;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Services.Auth;

public class ResourceManagerAuthorizationHandler : AuthorizationHandler<ResourceManagerRequirement, IUserManagedResource>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ResourceManagerRequirement requirement, IUserManagedResource resource)
    {
        if (context.User.IsInRole(ApplicationUserRoles.Admin) ||
            context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) == resource.OwnerId ||
            resource.Managers.Any(x => x.Id == context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

public record ResourceManagerRequirement : IAuthorizationRequirement;