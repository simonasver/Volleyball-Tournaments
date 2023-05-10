using System.Text.Json;
using Backend.Data.Dtos.Auth;
using Backend.Data.Dtos.User;
using Backend.Data.Entities.Auth;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Extensions;
using Backend.Helpers.Utils;
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
            LastLoginDate = DateTime.Now,
            Banned = false
        };
        var createUserResult = await _userManager.CreateAsync(newUser, userRegisterDto.Password);
        if (!createUserResult.Succeeded)
        {
            return BadRequest(createUserResult.Errors.FirstOrDefault()?.Description ?? "Could not create a user");
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

    [Authorize]
    [HttpGet("/api/[controller]", Name = "GetUsers")]
    public async Task<IActionResult> GetAll([FromQuery] SearchParameters searchParameters)
    {
        var users = await PagedList<ApplicationUser>.CreateAsync(_userManager.Users.Where(x => x.UserName.Contains(searchParameters.SearchInput)), searchParameters.PageNumber, searchParameters.PageSize);
        
        var previousPageLink = users.HasPrevious
            ? Url.Link("GetUsers", new
            {
                pageNumber = searchParameters.PageNumber - 1,
                pageSize = searchParameters.PageSize
            })
            : null;

        var nextPageLink = users.HasNext
            ? Url.Link("GetUsers", new
            {
                pageNumber = searchParameters.PageNumber + 1,
                pageSize = searchParameters.PageSize
            })
            : null;
        
        var paginationMetadata = new
        {
            totalCount = users.TotalCount,
            pageSize = users.PageSize,
            currentPage = users.CurrentPage,
            totalPages = users.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        List<GetUsersUserDto> usersDto = new List<GetUsersUserDto>();
        
        foreach (var user in users)
        {
            usersDto.Add(new GetUsersUserDto(user.Id, user.ProfilePictureUrl ?? "", user.UserName, user.FullName, user.Email, user.RegisterDate, user.LastLoginDate, await _userManager.GetRolesAsync(user), user.Banned));
        }

        return Ok(usersDto);
    }

    [Authorize]
    [HttpGet("/api/[controller]/{userId}")]
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

        return Ok(new GetUserDto(user.ProfilePictureUrl ?? "", user.UserName, user.FullName, user.Email, user.RegisterDate, user.LastLoginDate, roles, user.Banned));
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

        if (!String.IsNullOrEmpty(editUserDto.ProfilePictureUrl))
        {
            if (!(await Utils.IsLinkImage(editUserDto.ProfilePictureUrl)))
            {
                return BadRequest("Provided picture url was not an image");
            }
            userToEdit.ProfilePictureUrl = editUserDto.ProfilePictureUrl;
        }

        if (editUserDto.FullName != null && !string.IsNullOrEmpty(editUserDto.FullName))
        {
            userToEdit.FullName = editUserDto.FullName;
        }

        await _userManager.UpdateAsync(userToEdit);

        return Ok();
    }

    [Authorize(Roles = ApplicationUserRoles.Admin)]
    [HttpPatch("/api/[controller]/{userId}/Banned")]
    public async Task<IActionResult> Ban(string userId, [FromBody] BanDto banDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        user.Banned = banDto.Ban;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [Authorize(Roles = ApplicationUserRoles.Admin)]
    [HttpPatch("/api/[controller]/{userId}/Roles")]
    public async Task<IActionResult> AddRemoveRole(string userId, [FromBody] AddRemoveRoleDto addRemoveRoleDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        if (addRemoveRoleDto.Role == ApplicationUserRoles.User)
        {
            return BadRequest("Cannot remove or add User role");
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        if (addRemoveRoleDto.Add)
        {
            if (userRoles.Contains(addRemoveRoleDto.Role))
            {
                return BadRequest("User already has the role " + addRemoveRoleDto.Role);
            }
            await _userManager.AddToRoleAsync(user, addRemoveRoleDto.Role);
        }
        else
        {
            if (!userRoles.Contains(addRemoveRoleDto.Role))
            {
                return BadRequest("User is not in the role " + addRemoveRoleDto.Role);
            }

            await _userManager.RemoveFromRoleAsync(user, addRemoveRoleDto.Role);
        }
        
        return NoContent();
    }
}