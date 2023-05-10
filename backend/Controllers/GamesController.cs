using System.Text.Json;
using Backend.Data.Dtos.Game;
using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Auth;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ITeamService _teamService;
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogService _logService;

    public GamesController(IGameService gameService, ITeamService teamService, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, ILogService logService)
    {
        _gameService = gameService;
        _teamService = teamService;
        _authorizationService = authorizationService;
        _userManager = userManager;
        _logService = logService;
    }
    
    [AllowAnonymous]
    [HttpGet("/api/[controller]", Name = "GetGames")]
    public async Task<IActionResult> GetAll([FromQuery] bool all, [FromQuery] SearchParameters searchParameters)
    {
        if (all)
        {
            if (User.Identity == null || !User.IsInRole(ApplicationUserRoles.Admin))
            {
                return Forbid();
            }
        }

        var gamesResult = await _gameService.GetAllAsync(all, searchParameters);

        if (!gamesResult.IsSuccess)
        {
            return StatusCode(gamesResult.ErrorStatus, gamesResult.ErrorMessage);
        }
        
        var games = (PagedList<Game>)gamesResult.Data!;

        var previousPageLink = games.HasPrevious
            ? Url.Link("GetGames", new
            {
                pageNumber = searchParameters.PageNumber - 1,
                pageSize = searchParameters.PageSize
            })
            : null;

        var nextPageLink = games.HasNext
            ? Url.Link("GetGames", new
            {
                pageNumber = searchParameters.PageNumber + 1,
                pageSize = searchParameters.PageSize
            })
            : null;
    
        var paginationMetadata = new
        {
            totalCount = games.TotalCount,
            pageSize = games.PageSize,
            currentPage = games.CurrentPage,
            totalPages = games.TotalPages,
            previousPageLink,
            nextPageLink
        };
    
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
        
        return Ok(games);
    }

    [Authorize]
    [HttpGet("/api/Users/{userId}/[controller]", Name = "GetUserGames")]
    public async Task<IActionResult> GetUserGames(string userId, [FromQuery] SearchParameters searchParameters)
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
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            if (user.Id != userId)
            {
                return Forbid();
            }
        }

        var userGamesResult = await _gameService.GetUserGamesAsync(searchParameters, userId);

        if (!userGamesResult.IsSuccess)
        {
            return StatusCode(userGamesResult.ErrorStatus, userGamesResult.ErrorMessage);
        }

        var userGames = (PagedList<Game>)userGamesResult.Data!;
        
        var previousPageLink = userGames.HasPrevious
            ? Url.Link("GetUserGames", new
            {
                pageNumber = searchParameters.PageNumber - 1,
                pageSize = searchParameters.PageSize
            })
            : null;

        var nextPageLink = userGames.HasNext
            ? Url.Link("GetUserGames", new
            {
                pageNumber = searchParameters.PageNumber + 1,
                pageSize = searchParameters.PageSize
            })
            : null;
        
        var paginationMetadata = new
        {
            totalCount = userGames.TotalCount,
            pageSize = userGames.PageSize,
            currentPage = userGames.CurrentPage,
            totalPages = userGames.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
        
        return Ok(userGames);
    }

    [AllowAnonymous]
    [HttpGet("/api/[controller]/{gameId}")]
    public async Task<IActionResult> Get(Guid gameId)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;

        if (game == null)
        {
            return NotFound();
        }

        if (game.IsPrivate)
        {
            // Only if it's user owned resource, user is manager or user is admin
            if (!User.IsInRole(ApplicationUserRoles.Admin))
            {
                var authorization =
                    await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
                if (!authorization.Succeeded)
                {
                    return NotFound();
                }
            }
        }

        return Ok(game);
    }

    [Authorize]
    [HttpPost("/api/[controller]")]
    public async Task<IActionResult> Post([FromBody] AddGameDto addGameDto)
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

        var createdGameResult = await _gameService.CreateAsync(addGameDto, user.Id);

        if (!createdGameResult.IsSuccess)
        {
            return StatusCode(createdGameResult.ErrorStatus, createdGameResult.ErrorMessage);
        }

        return CreatedAtAction(nameof(Post), createdGameResult.Data!.Id);
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}")]
    public async Task<IActionResult> Patch(Guid gameId, [FromBody] EditGameDto editGameDto)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;

        if (game == null)
        {
            return NotFound();
        }

        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.UpdateAsync(editGameDto, game);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("/api/[controller]/{gameId}")]
    public async Task<IActionResult> Delete(Guid gameId)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;

        if (game == null)
        {
            return NotFound();
        }

        // Only if it's user owned resource, user is manager, or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.DeleteAsync(game);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return NoContent();
    }

    [Authorize]
    [HttpPost("/api/[controller]/{gameId}/RequestedTeams")]
    public async Task<IActionResult> TeamRequestJoin(Guid gameId, [FromBody] RequestJoinGameDto requestJoinGameDto)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;

        if (game == null)
        {
            return NotFound();
        }

        var teamResult = await _teamService.GetAsync(requestJoinGameDto.TeamId);

        if (!teamResult.IsSuccess)
        {
            return StatusCode(teamResult.ErrorStatus, teamResult.ErrorMessage);
        }

        var team = teamResult.Data;

        if (team == null)
        {
            return NotFound();
        }
        
        // Only if user is team owner, manager or is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, team, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.TeamRequestJoinAsync(game, team);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return Ok();
    }

    [Authorize]
    [HttpPost("/api/[controller]/{gameId}/GameTeams")]
    public async Task<IActionResult> AddTeam(Guid gameId, [FromBody] AddTeamToGameDto addTeamToGameDto)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;

        if (game == null)
        {
            return NotFound();
        }

        // Only if it's user owned resource, user is manager, or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.AddTeamAsync(addTeamToGameDto, game);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return Ok(result.Data);
    }

    [Authorize]
    [HttpDelete("/api/[controller]/{gameId}/GameTeams")]
    public async Task<IActionResult> RemoveTeam(Guid gameId, [FromQuery] bool team)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;

        if (game == null)
        {
            return NotFound();
        }

        // Only if it's user owned resource, is manager or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.RemoveTeamAsync(team, game);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}/Status")]
    public async Task<IActionResult> Start(Guid gameId)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;

        if (game == null)
        {
            return NotFound();
        }
        
        var user = await _userManager.FindByNameAsync(User.Identity?.Name);
        
        if (user == null)
        {
            return Forbid();
        }
        
        // Only if it's user owned resource, user is manager or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.StartAsync(game);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return Ok();
    }
    
    [AllowAnonymous]
    [HttpGet("/api/[controller]/{gameId}/Sets")]
    public async Task<IActionResult> GetGameSets(Guid gameId)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;

        if (game == null)
        {
            return BadRequest("Game does not exist");
        }
    
        if (game.IsPrivate)
        {
            // Only if it's user owned resource, user is manager or user is admin
            if (!User.IsInRole(ApplicationUserRoles.Admin))
            {
                var authorization =
                    await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
                if (!authorization.Succeeded)
                {
                    return NotFound();
                }
            }
        }

        var result = await _gameService.GetGameSetsAsync(gameId);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}/Sets/{setId}/Players/{playerId}/Score")]
    public async Task<IActionResult> ChangeSetPlayerScore(Guid gameId, Guid setId, Guid playerId, [FromBody] ChangeSetPlayerScoreDto changeSetPlayerScoreDto)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;
        
        if (game == null)
        {
            return BadRequest("Game does not exist");
        }
        
        var user = await _userManager.FindByNameAsync(User.Identity?.Name);
        
        if (user == null)
        {
            return Forbid();
        }

        // Only if it's user owned resource, user is manager or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.ChangePlayerSetScoreAsync(changeSetPlayerScoreDto, game, setId, playerId, user.Id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return NoContent();
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}/Sets/{setId}/Players/{playerId}/Stats")]
    public async Task<IActionResult> ChangeSetPlayerStats(Guid gameId, Guid setId, Guid playerId,
        [FromBody] ChangeSetPlayerStatsDto changeSetPlayerStatsDto)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;
        
        if (game == null)
        {
            return BadRequest("Game does not exist");
        }
        
        var user = await _userManager.FindByNameAsync(User.Identity?.Name);
        
        if (user == null)
        {
            return Forbid();
        }
        
        // Only if it's user owned resource, user is manager or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.ChangePlayerSetStatsAsync(changeSetPlayerStatsDto, game, setId, playerId, user.Id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("/api/[controller]/{gameId}/Logs")]
    public async Task<IActionResult> GetLogs(Guid gameId)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data;
        
        if (game == null)
        {
            return BadRequest("Game does not exist");
        }
        
        var logs = await _logService.GetGameLogsAsync(gameId);

        if (!logs.IsSuccess)
        {
            return StatusCode(logs.ErrorStatus, logs.ErrorMessage);
        }
        
        // Only if it's user owned resource, user is manager or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceManager);
            if (!authorization.Succeeded)
            {
                return Ok(logs.Data!.Where(x => !x.IsPrivate));
            }

            return Ok(logs.Data);
        }

        return Ok(logs.Data);
    }
    
    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}/Managers")]
    public async Task<IActionResult> AddManager(Guid gameId, [FromBody] AddManagerDto addManagerDto)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data!;
        
        // Only if it's user owned resource, or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var userForManager = await _userManager.FindByIdAsync(addManagerDto.ManagerId);

        var result = await _gameService.AddManager(game, userForManager);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("/api/[controller]/{gameId}/Managers/{managerId}")]
    public async Task<IActionResult> RemoveManager(Guid gameId, string managerId)
    {
        var gameResult = await _gameService.GetAsync(gameId);

        if (!gameResult.IsSuccess)
        {
            return StatusCode(gameResult.ErrorStatus, gameResult.ErrorMessage);
        }

        var game = gameResult.Data!;
        
        // Only if it's user owned resource, or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var userForManager = await _userManager.FindByIdAsync(managerId);

        var result = await _gameService.RemoveManager(game, userForManager);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }
}

