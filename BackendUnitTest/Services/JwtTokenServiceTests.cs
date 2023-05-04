using Backend.Auth.Model;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.Extensions.Configuration;

namespace TestProject.Services;

public class JwtTokenServiceTests
{
    private IJwtTokenService _jwtTokenService;
    
    [SetUp]
    public void Setup()
    {
        var configuration = new Dictionary<string, string>
        {
            { "JWT:Secret", "5vf68W7DAkoVAAA5xLZG3I7wKZO3oC7G5sFVNx98" },
            { "JWT:ValidIssuer", "https://localhost:44348/" },
            { "JWT:ValidAudience", "https://localhost:44348/" },
            { "JWT:AccessTokenValidityInMinutes", "5" },
            { "JWT:RefreshTokenValidityInDays", "7" }
        };
        var config = new ConfigurationManager().AddInMemoryCollection(configuration).Build();
        _jwtTokenService = new JwtTokenService(config);
    }

    [Test]
    public void CreateAccessToken_ReturnsToken()
    {
        var result = _jwtTokenService.CreateAccessToken("user", "user", new[] { ApplicationUserRoles.User });
        
        Assert.AreEqual(typeof(String), result.GetType());
    }

    [Test]
    public void CreateRefreshToken_ReturnsTokenAndDate()
    {
        var result = _jwtTokenService.CreateRefreshToken();
        
        Assert.AreEqual(result.RefreshTokenExpiration.Day, DateTime.Now.AddDays(7).Day);
        Assert.AreEqual(typeof(String), result.RefreshToken.GetType());
    }

    [Test]
    public void GetPrincipalFromExpiredToken_ReturnsPrincipal()
    {
        var token = _jwtTokenService.CreateAccessToken("user", "user", new[] { ApplicationUserRoles.User });
        
        var result = _jwtTokenService.GetPrincipalFromExpiredToken(token);
        
        Assert.AreEqual(result.Identities.First().Name, "user");
    }
}