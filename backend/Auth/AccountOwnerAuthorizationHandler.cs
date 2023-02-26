using System.Security.Claims;
using Backend.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Backend.Auth
{
    public class AccountOwnerAuthorizationHandler : AuthorizationHandler<AccountOwnerRequirement, IUserOwnedUser>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountOwnerRequirement requirement, IUserOwnedUser resource)
        {
            if (context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) == resource.Id)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public record AccountOwnerRequirement : IAuthorizationRequirement;
}
