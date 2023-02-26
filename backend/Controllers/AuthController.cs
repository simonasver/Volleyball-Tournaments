using Backend.Auth.Model;
using Backend.Data.Dtos.Auth;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> Register(RegisterDto userRegisterDto)
        {
            var user = await _userManager.FindByNameAsync(userRegisterDto.UserName);
            if (user != null)
            {
                return BadRequest("Request invalid");
            }

            var newUser = new ApplicationUser
            {
                Email = userRegisterDto.Email,
                UserName = userRegisterDto.UserName,
                FullName = userRegisterDto.UserName,
                RegisterDate = DateTime.Now,
                LastLoginDate = DateTime.Now
            };
            var createUserResult = await _userManager.CreateAsync(newUser, userRegisterDto.Password);
            if (!createUserResult.Succeeded)
            {
                return BadRequest("Could not create a user");
            }

            await _userManager.AddToRoleAsync(newUser, ApplicationUserRoles.User);

            var newUserDto = new UserDto()
            {
                Id = newUser.Id,
                UserName = newUser.UserName,
                Email = newUser.Email
            };

            return CreatedAtAction(nameof(Register), newUserDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDtoRequest userLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userLoginDto.UserName);
            if (user == null)
            {
                return BadRequest("ApplicationUser name or password is invalid");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
            if (!isPasswordValid)
            {
                return BadRequest("ApplicationUser name or password is invalid");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            var refreshToken = _jwtTokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken.RefreshToken;
            user.RefreshTokenExpiration = refreshToken.RefreshTokenExpiration;

            user.LastLoginDate = DateTime.Now;

            await _userManager.UpdateAsync(user);

            return Ok(new LoginDtoResponse(accessToken, refreshToken.RefreshToken, user.UserName, user.Email, roles));
        }

        [HttpPut("refresh")]
        public async Task<IActionResult> Refresh(TokenDto tokenDto)
        {
            if (tokenDto == null)
            {
                return BadRequest();
            }

            string? accessToken = tokenDto.AccessToken;
            string? refreshToken = tokenDto.RefreshToken;

            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest();
            }

            string username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if(user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiration <= DateTime.Now)
            {
                return BadRequest();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var newAccessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            var newRefreshToken = _jwtTokenService.CreateRefreshToken();

            user.RefreshToken = newRefreshToken.RefreshToken;
            user.RefreshTokenExpiration = newRefreshToken.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return CreatedAtAction(nameof(Refresh), new TokenDto(newAccessToken, newRefreshToken.RefreshToken));
        }

        [Authorize(Policy = PolicyNames.AccountOwner)]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest();
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiration = DateTime.Now;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }
    }
}
