using System.Security.Claims;
using Backend.Data.Dtos.Auth;

namespace Backend.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(string userName, string userId, IEnumerable<string> userRoles);
        RefreshTokenDto CreateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
