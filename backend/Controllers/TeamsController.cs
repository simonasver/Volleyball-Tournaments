using System.Text.Json;
using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Auth;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITeamService _teamService;

    public TeamsController(IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, ITeamService teamService)
    {
        _authorizationService = authorizationService;
        _userManager = userManager;
        _teamService = teamService;
    }

    [Authorize(Roles = ApplicationUserRoles.Admin)]
    [HttpGet("/api/[controller]", Name = "GetTeams")]
    public async Task<IActionResult> GetAll([FromQuery] SearchParameters searchParameters)
    {
        var teamsResult = await _teamService.GetAllAsync(searchParameters);

        if (!teamsResult.IsSuccess)
        {
            return StatusCode(teamsResult.ErrorStatus, teamsResult.ErrorMessage);
        }

        var teams = (PagedList<Team>)teamsResult.Data!;

        var previousPageLink = teams.HasPrevious
            ? Url.Link("GetTeams", new
            {
                pageNumber = searchParameters.PageNumber - 1,
                pageSize = searchParameters.PageSize
            })
            : null;

        var nextPageLink = teams.HasNext
            ? Url.Link("GetTeams", new
            {
                pageNumber = searchParameters.PageNumber + 1,
                pageSize = searchParameters.PageSize
            })
            : null;
        
        var paginationMetadata = new
        {
            totalCount = teams.TotalCount,
            pageSize = teams.PageSize,
            currentPage = teams.CurrentPage,
            totalPages = teams.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(teams);
    }

    [Authorize]
    [HttpGet("/api/Users/{userId}/[controller]", Name = "GetUserTeams")]
    public async Task<IActionResult> GetUserTeams([FromQuery] SearchParameters searchParameters, string userId)
    {
        if (User.Identity == null)
        {
            return Forbid();
        }

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        if (user == null)
        {
            return Forbid();
        }
        
        // Only if it's user owned resource, or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            if (user.Id != userId)
            {
                return Forbid();
            }
        }
        
        var userTeamsResult = await _teamService.GetUserTeamsAsync(searchParameters, userId);
        if (!userTeamsResult.IsSuccess)
        {
            return StatusCode(userTeamsResult.ErrorStatus, userTeamsResult.ErrorMessage);
        }
        var userTeams = (PagedList<Team>)userTeamsResult.Data!;
        
        var previousPageLink = userTeams.HasPrevious
            ? Url.Link("GetUserTeams", new
            {
                pageNumber = searchParameters.PageNumber - 1,
                pageSize = searchParameters.PageSize
            })
            : null;

        var nextPageLink = userTeams.HasNext
            ? Url.Link("GetUserTeams", new
            {
                pageNumber = searchParameters.PageNumber + 1,
                pageSize = searchParameters.PageSize
            })
            : null;
        
        var paginationMetadata = new
        {
            totalCount = userTeams.TotalCount,
            pageSize = userTeams.PageSize,
            currentPage = userTeams.CurrentPage,
            totalPages = userTeams.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
        
        return Ok(userTeams);
    }
    
    [AllowAnonymous]
    [HttpGet("/api/[controller]/{teamId}")]
    public async Task<IActionResult> Get(Guid teamId)
    {
        var teamResult = await _teamService.GetAsync(teamId);

        if (teamResult.IsSuccess)
        {
            return Ok(teamResult.Data);
        }

        return StatusCode(teamResult.ErrorStatus, teamResult.ErrorMessage);
    }

    [Authorize]
    [HttpPost("/api/[controller]")]
    public async Task<IActionResult> Post([FromBody] AddTeamDto addTeamDto)
    {
        if (User.Identity == null)
        {
            return Forbid();
        }

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        if (user == null)
        {
            return Forbid();
        }

        var createdTeamResult = await _teamService.CreateAsync(addTeamDto, user.Id);

        if (createdTeamResult.IsSuccess)
        {
            return CreatedAtAction(nameof(Post), createdTeamResult.Data!.Id);
        }

        return StatusCode(createdTeamResult.ErrorStatus, createdTeamResult.ErrorMessage);
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{teamId}")]
    public async Task<IActionResult> Patch(Guid teamId, [FromBody] EditTeamDto editTeamDto)
    {
        var teamResult = await _teamService.GetAsync(teamId);

        if (!teamResult.IsSuccess)
        {
            return StatusCode(teamResult.ErrorStatus, teamResult.ErrorMessage);
        }

        var team = teamResult.Data;
        
        if (team == null)
        {
            return NotFound();
        }
        
        // Only if it's user owned resource, user is manager, or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, team, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _teamService.UpdateAsync(editTeamDto, team);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("/api/[controller]/{teamId}")]
    public async Task<IActionResult> Delete(Guid teamId)
    {
        var teamResult = await _teamService.GetAsync(teamId);

        if (!teamResult.IsSuccess)
        {
            return StatusCode(teamResult.ErrorStatus, teamResult.ErrorMessage);
        }

        var team = teamResult.Data;
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, team, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _teamService.DeleteAsync(teamId);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return NoContent();
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{teamId}/Players")]
    public async Task<IActionResult> AddPlayer(Guid teamId, [FromBody] AddTeamPlayerDto addTeamPlayerDto)
    {
        var teamResult = await _teamService.GetAsync(teamId);

        if (!teamResult.IsSuccess)
        {
            return StatusCode(teamResult.ErrorStatus, teamResult.ErrorMessage);
        }

        var team = teamResult.Data!;
        
        // Only if it's user owned resource, user is manager or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, team, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _teamService.AddPlayerAsync(addTeamPlayerDto, team);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("/api/[controller]/{teamId}/Players/{playerId}")]
    public async Task<IActionResult> RemovePlayer(Guid teamId, Guid playerId)
    {
        var teamResult = await _teamService.GetAsync(teamId);

        if (!teamResult.IsSuccess)
        {
            return StatusCode(teamResult.ErrorStatus, teamResult.ErrorMessage);
        }

        var team = teamResult.Data!;
        
        // Only if it's user owned resource, user is manager or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, team, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _teamService.RemovePlayerAsync(playerId, team);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return NoContent();
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{teamId}/Managers")]
    public async Task<IActionResult> AddManager(Guid teamId, [FromBody] AddManagerDto addManagerDto)
    {
        var teamResult = await _teamService.GetAsync(teamId);

        if (!teamResult.IsSuccess)
        {
            return StatusCode(teamResult.ErrorStatus, teamResult.ErrorMessage);
        }

        var team = teamResult.Data!;
        
        // Only if it's user owned resource, or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, team, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var userForManager = await _userManager.FindByIdAsync(addManagerDto.ManagerId);

        var result = await _teamService.AddManagerAsync(team, userForManager);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("/api/[controller]/{teamId}/Managers/{managerId}")]
    public async Task<IActionResult> RemoveManager(Guid teamId, string managerId)
    {
        var teamResult = await _teamService.GetAsync(teamId);

        if (!teamResult.IsSuccess)
        {
            return StatusCode(teamResult.ErrorStatus, teamResult.ErrorMessage);
        }

        var team = teamResult.Data!;
        
        // Only if it's user owned resource, or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, team, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var userForManager = await _userManager.FindByIdAsync(managerId);

        var result = await _teamService.RemoveManagerAsync(team, userForManager);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }
}
