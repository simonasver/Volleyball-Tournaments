using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Auth.Model;
using Backend.Data.Dtos.Game;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Helpers.Utils;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly IGameService _gameService;
    private readonly ITeamService _teamService;
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogService _logService;

    public GamesController(ITournamentService tournamentService, IGameService gameService, ITeamService teamService, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, ILogService logService)
    {
        _tournamentService = tournamentService;
        _gameService = gameService;
        _teamService = teamService;
        _authorizationService = authorizationService;
        _userManager = userManager;
        _logService = logService;
    }
    
    [AllowAnonymous]
    [HttpGet("/api/[controller]")]
    public async Task<IActionResult> GetAll([FromQuery] bool all)
    {
        var gamesResult = await _gameService.GetAllAsync();

        if (gamesResult.IsSuccess)
        {
            if (all)
            {
                if (User.Identity == null || !User.IsInRole(ApplicationUserRoles.Admin))
                {
                    return Forbid();
                }

                return Ok(gamesResult.Data);
            }
            else
            {
                return Ok(gamesResult.Data.Where(x => !x.IsPrivate && x.TournamentMatch == null).ToList()); 
            }
        }
        else
        {
            return StatusCode(gamesResult.ErrorStatus, gamesResult.ErrorMessage);
        }
    }

    [Authorize]
    [HttpGet("/api/Users/{userId}/[controller]")]
    public async Task<IActionResult> GetUserGames(string userId)
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

        var userGamesResult = await _gameService.GetUserGamesAsync(userId);

        if (!userGamesResult.IsSuccess)
        {
            return StatusCode(userGamesResult.ErrorStatus, userGamesResult.ErrorMessage);
        }
        else
        {
            return Ok(userGamesResult.Data);
        }
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

        if (game.IsPrivate == true)
        {
            // Only if it's user owned resource or user is admin
            if (!User.IsInRole(ApplicationUserRoles.Admin))
            {
                var authorization =
                    await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
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

        return CreatedAtAction(nameof(Post), createdGameResult.Data.Id);
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
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
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

        // Only if it's user owned resource or user is admin
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
        
        // Only if user is team owner or is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, team, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.TeamRequestJoinAsync(requestJoinGameDto, game, team);

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

        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
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

        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
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
        
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        if (user == null)
        {
            return Forbid();
        }
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
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
    
        if (game.IsPrivate == true)
        {
            // Only if it's user owned resource or user is admin
            if (!User.IsInRole(ApplicationUserRoles.Admin))
            {
                var authorization =
                    await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
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
        
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        if (user == null)
        {
            return Forbid();
        }

        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        var result = await _gameService.ChangePlayerSetScoreAsync(changeSetPlayerScoreDto, game, setId, playerId, user.Id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorStatus);
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
        
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        if (user == null)
        {
            return Forbid();
        }
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
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
        
        var logs = await _logService.GetGameLogs(gameId);

        if (!logs.IsSuccess)
        {
            return StatusCode(logs.ErrorStatus, logs.ErrorMessage);
        }
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, game, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Ok(logs.Data.Where(x => !x.IsPrivate));
            }
            else
            {
                return Ok(logs.Data);
            }
        }
        else
        {
            return Ok(logs.Data);
        }
    }
}

