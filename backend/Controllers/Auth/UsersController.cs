using Backend.Auth.Model;
using Backend.Data.Dtos.Auth;
using Backend.Data.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto userRegisterDto)
    {
        var user = await _userManager.FindByNameAsync(userRegisterDto.UserName);
        if (user != null)
        {
            return BadRequest("This user already exists");
        }

        if (await _userManager.FindByEmailAsync(userRegisterDto.Email) != null)
        {
            return BadRequest("This email is already in use");
        }

        var newUser = new ApplicationUser
        {
            Email = userRegisterDto.Email,
            UserName = userRegisterDto.UserName,
            FullName = userRegisterDto.FullName ?? userRegisterDto.UserName,
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

    [HttpGet("/api/[controller]/{userId}")]
    [Authorize]
    public async Task<IActionResult> Get(string userId)
    {
        if (User.Identity == null)
        {
            return Forbid();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Forbid();
        }

        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            if ((await _userManager.FindByNameAsync(User.Identity.Name)).Id != userId)
            {
                return Forbid();
            }
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new GetUserDto(user.ProfilePictureUrl ?? "", user.UserName, user.FullName, user.Email, user.RegisterDate, user.LastLoginDate, roles));

    }
    
    [Authorize]
    [HttpPatch("/api/[controller]/{userId}")]
    public async Task<IActionResult> Edit(string userId, [FromBody] EditUserDto editUserDto)
    {
        if (User.Identity == null)
        {
            return Forbid();
        }
        
        var userToEdit = await _userManager.FindByIdAsync(userId);
        if (userToEdit == null)
        {
            return NotFound();
        }

        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            if ((await _userManager.FindByNameAsync(User.Identity.Name)).Id != userId)
            {
                return Forbid();
            }
        }

        if (editUserDto.ProfilePictureUrl != null)
        {
            // Check if its actually a picture
            userToEdit.ProfilePictureUrl = editUserDto.ProfilePictureUrl;
        }

        if (editUserDto.FullName != null && !string.IsNullOrEmpty(editUserDto.FullName))
        {
            userToEdit.FullName = editUserDto.FullName;
        }

        await _userManager.UpdateAsync(userToEdit);

        return Ok();
    }
}