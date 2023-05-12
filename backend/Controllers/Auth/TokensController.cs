using Backend.Data.Dtos.Auth;
using Backend.Data.Entities.Auth;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class TokensController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    public TokensController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<string>> Login([FromBody] LoginDtoRequest userLoginDto)
    {
        var user = await _userManager.FindByNameAsync(userLoginDto.UserName);
        if (user == null)
        {
            return BadRequest("User Name or password is invalid");
        }
        
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!isPasswordValid)
        {
            return BadRequest("User Name or password is invalid");
        }

        if (user.Banned)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "User does not have access to the system");
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
        var refreshToken = _jwtTokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken.RefreshToken;
        user.RefreshTokenExpiration = refreshToken.RefreshTokenExpiration;

        user.LastLoginDate = DateTime.Now;

        await _userManager.UpdateAsync(user);

        return CreatedAtAction(nameof(Login), new LoginDtoResponse(accessToken, refreshToken.RefreshToken, user.Id, user.ProfilePictureUrl ?? "", user.UserName, user.FullName, user.Email, roles));
    }

    [AllowAnonymous]
    [HttpPut]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
    {
        string accessToken = tokenDto.AccessToken;
        string refreshToken = tokenDto.RefreshToken;

        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return BadRequest("Bad principal");
        }

        string? username = principal.Identity?.Name;

        if (username == null)
        {
            return BadRequest("Bad username");
        }

        var user = await _userManager.FindByNameAsync(username);

        if(user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiration <= DateTime.Now)
        {
            return BadRequest("Bad user or token");
        }
        
        if (user.Banned)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "User does not have access to the system");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
        var newRefreshToken = _jwtTokenService.CreateRefreshToken();

        user.RefreshToken = newRefreshToken.RefreshToken;
        user.RefreshTokenExpiration = newRefreshToken.RefreshTokenExpiration;
        
        await _userManager.UpdateAsync(user);

        return Ok(new TokenDto(newAccessToken, newRefreshToken.RefreshToken));
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity == null)
        {
            return Forbid();
        }
        
        var userName = User.Identity.Name;
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            return Forbid();
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiration = DateTime.Now;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}
