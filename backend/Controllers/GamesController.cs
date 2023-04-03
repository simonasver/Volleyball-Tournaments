﻿using System.Diagnostics;
using Backend.Auth.Model;
using Backend.Data.Dtos.Game;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
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
    private readonly IGameService _gameService;
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGameRepository _gameRepository;
    private readonly ISetRepository _setRepository;
    private readonly ITeamRepository _teamRepository;

    public GamesController(IGameService gameService, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IGameRepository gameRepository, ISetRepository setRepository, ITeamRepository teamRepository)
    {
        _gameService = gameService;
        _authorizationService = authorizationService;
        _userManager = userManager;
        _gameRepository = gameRepository;
        _setRepository = setRepository;
        _teamRepository = teamRepository;
    }
    
    [AllowAnonymous]
    [HttpGet("/api/[controller]")]
    public async Task<IActionResult> GetAll([FromQuery] bool all)
    {
        IEnumerable<Game> games;
        if (all)
        {
            if (User.Identity == null || !User.IsInRole(ApplicationUserRoles.Admin))
            {
                return Forbid();
            }

            games = await _gameRepository.GetAllAsync();
        }
        else
        {
            games = (await _gameRepository.GetAllAsync()).Where(x => !x.IsPrivate).ToList();
;            }

        return Ok(games);
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

        var games = await _gameRepository.GetAllAsync();
        var userGames = games.Where(x => x.OwnerId == user.Id).ToList();

        return Ok(userGames);
    }

    [AllowAnonymous]
    [HttpGet("/api/[controller]/{gameId}")]
    public async Task<IActionResult> Get(Guid gameId)
    {
        var game = await _gameRepository.GetAsync(gameId);

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
        
        if (addGameDto.MaxSets % 2 == 0)
        {
            return BadRequest("Max sets must be an odd number");
        }

        var newGame = new Game
        {
            Title = addGameDto.Title,
            Description = addGameDto.Description,
            PointsToWin = addGameDto.PointsToWin,
            PointDifferenceToWin = addGameDto.PointDifferenceToWin,
            MaxSets = addGameDto.MaxSets,
            PlayersPerTeam = addGameDto.PlayersPerTeam,
            IsPrivate = addGameDto.IsPrivate,
            OwnerId = user.Id,
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            FirstTeamScore = 0,
            SecondTeamScore = 0,
        };

        var createdGame = await _gameRepository.CreateAsync(newGame);
        
        return CreatedAtAction(nameof(Post), createdGame.Id);
    }

    [Authorize]
    [HttpPut("/api/[controller]/{gameId}")]
    public async Task<IActionResult> Patch(Guid gameId, [FromBody] EditGameDto editGameDto)
    {
        var game = await _gameRepository.GetAsync(gameId);

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

        if (game.Status == GameStatus.Finished)
        {
            return BadRequest("Cannot edit finished game");
        }

        if (editGameDto.MaxSets % 2 == 0)
        {
            return BadRequest("Max sets must be an odd number");
        }

        if (editGameDto.Title != null)
        {
            game.Title = editGameDto.Title;
        }

        if (editGameDto.Description != null)
        {
            game.Description = editGameDto.Description;
        }

        if (editGameDto.PointsToWin != null)
        {
            if (game.Status < GameStatus.Started)
            {
                game.PointsToWin = editGameDto.PointsToWin ?? game.PointsToWin;
            }
        }

        if (editGameDto.PointDifferenceToWin != null)
        {
            if (game.Status < GameStatus.Started)
            {
                game.PointDifferenceToWin = editGameDto.PointDifferenceToWin ?? game.PointDifferenceToWin;
            }
        }

        if (editGameDto.MaxSets != null)
        {
            if (game.Status < GameStatus.Started)
            {
                game.MaxSets = editGameDto.MaxSets ?? game.MaxSets;
            }
        }
        
        if (editGameDto.PlayersPerTeam != null)
        {
            if (game.Status == GameStatus.New)
            {
                game.PlayersPerTeam = editGameDto.PlayersPerTeam ?? game.PlayersPerTeam;
            }
        }

        if (editGameDto.IsPrivate != null)
        {
            game.IsPrivate = editGameDto.IsPrivate ?? game.IsPrivate;
        }
        
        game.LastEditDate = DateTime.Now;

        await _gameRepository.UpdateAsync(game);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("/api/[controller]/{gameId}")]
    public async Task<IActionResult> Delete(Guid gameId)
    {
        var game = await _gameRepository.GetAsync(gameId);

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

        await _gameRepository.DeleteAsync(gameId);

        return NoContent();
    }

    [Authorize]
    [HttpPost("/api/[controller]/{gameId}/RequestedTeams")]
    public async Task<IActionResult> RequestTeam(Guid gameId, [FromBody] RequestJoinGameDto requestJoinGameDto)
    {
        var game = await _gameRepository.GetAsync(gameId);

        if (game == null)
        {
            return NotFound();
        }

        var team = await _teamRepository.GetAsync(requestJoinGameDto.TeamId);

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

        if (game.RequestedTeams.Any(x => x.Id == team.Id) || (game.FirstTeam != null && game.FirstTeam.Title == team.Title) ||
            (game.SecondTeam != null && game.SecondTeam.Title == team.Title))
        {
            return BadRequest("Team already requested to join this game");
        }

        if (team.Players.Count != game.PlayersPerTeam && game.PlayersPerTeam != 0)
        {
            return BadRequest("Teams in this game are required to have " + game.PlayersPerTeam + " players");
        }

        if (team.Players.Count == 0)
        {
            return BadRequest("Team must have at least 1 player");
        }

        game.RequestedTeams.Add(team);

        await _gameRepository.UpdateAsync(game);

        return Ok();
    }

    [Authorize]
    [HttpPost("/api/[controller]/{gameId}/GameTeams")]
    public async Task<IActionResult> AddTeam(Guid gameId, [FromBody] AddTeamToGameDto addTeamToGameDto)
    {
        var game = await _gameRepository.GetAsync(gameId);

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

        if (!game.RequestedTeams.Any(x => x.Id == addTeamToGameDto.TeamId))
        {
            return BadRequest("Team has not requested to join the game");
        }

        var team = game.RequestedTeams.FirstOrDefault(x => x.Id == addTeamToGameDto.TeamId);
        
        if ((team.Players.Count != game.PlayersPerTeam && game.PlayersPerTeam != 0) || team.Players.Count == 0)
        {
            game.RequestedTeams.Remove(team);
            await _gameRepository.UpdateAsync(game);
            return BadRequest("Team has 0 or another incompatible number of players for this game");
        }

        game = _gameService.AddTeamToGame(game, team);

        game.RequestedTeams.Remove(team);

        await _gameRepository.UpdateAsync(game);
        
        return Ok(game);
    }

    [Authorize]
    [HttpDelete("/api/[controller]/{gameId}/GameTeams")]
    public async Task<IActionResult> RemoveTeam(Guid gameId, [FromQuery] bool team)
    {
        var game = await _gameRepository.GetAsync(gameId);

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

        if (game.Status >= GameStatus.Started)
        {
            return BadRequest("Game is already started");
        }

        if (team == false)
        {
            if (game.FirstTeam == null)
            {
                return BadRequest("Game does not have a first team");
            }

            game.FirstTeam = null;
        }
        else
        {
            if (game.SecondTeam == null)
            {
                return BadRequest("Game does not have a second team");
            }

            game.SecondTeam = null;
        }
        
        if ((game.FirstTeam != null && game.SecondTeam == null) || (game.FirstTeam == null && game.SecondTeam != null))
        {
            game.Status = GameStatus.SingleTeam;
        }
        else if (game.FirstTeam == null && game.SecondTeam == null)
        {
            game.Status = GameStatus.New;
        }

        await _gameRepository.UpdateAsync(game);
        
        return NoContent();
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}/Status")]
    public async Task<IActionResult> Start(Guid gameId)
    {
        var game = await _gameRepository.GetAsync(gameId);

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

        if (game.Status == GameStatus.Started)
        {
            return BadRequest("Game is already started");
        }

        if (game.Status == GameStatus.Finished)
        {
            return BadRequest(("Game is already finished"));
        }

        if (game.Status != GameStatus.Ready)
        {
            return BadRequest("Game does not two teams");
        }

        if (game.FirstTeam.Players.Count != game.PlayersPerTeam && game.PlayersPerTeam != 0)
        {
            return BadRequest("First team has a different player count than needed (" + game.PlayersPerTeam + ")");
        }

        if (game.SecondTeam.Players.Count != game.PlayersPerTeam && game.PlayersPerTeam != 0)
        {
            return BadRequest("Second team has a different player count than needed (" + game.PlayersPerTeam + ")");
        }

        game.Status = GameStatus.Started;
        

        for (var i = 0; i < game.MaxSets; i++)
        {
            game = _gameService.AddSetToGame(game, i);
        }

        var firstSet = game.Sets.FirstOrDefault(x => x.Number == 1);
        firstSet.StartDate = DateTime.Now;
        firstSet.Status = GameStatus.Started;

        game.LastEditDate = DateTime.Now;
        game.StartDate = DateTime.Now;

        await _gameRepository.UpdateAsync(game);

        return Ok();
    }
    
    [AllowAnonymous]
    [HttpGet("/api/[controller]/{gameId}/Sets")]
    public async Task<IActionResult> GetGameSets(Guid gameId)
    {
        var game = await _gameRepository.GetAsync(gameId);

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

        var sets = (await _setRepository.GetAllAsync()).Where(x => x.Game.Id == gameId).OrderBy(x => x.Number).ToList();

        return Ok(sets);
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}/Sets/{setId}/Players/{playerId}")]
    public async Task<IActionResult> ChangeSetPlayerScore(Guid gameId, Guid setId, Guid playerId, [FromBody] ChangeSetPlayerScoreDto changeSetPlayerScoreDto)
    {
        var game = await _gameRepository.GetAsync(gameId);
        if (game == null)
        {
            return BadRequest("Game does not exist");
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

        var set = game.Sets.FirstOrDefault(x => x.Id == setId);
        if (set == null)
        {
            return BadRequest("Set does not exist");
        }

        if (set.Status == GameStatus.Finished)
        {
            return BadRequest("Set is already finished");
        }

        var player = set.Players.FirstOrDefault(x => x.Id == playerId);
        if (player == null)
        {
            return BadRequest("Player does not exist");
        }

        if (changeSetPlayerScoreDto.Change)
        {
            player.Score++;
            if (!player.Team)
            {
                set.FirstTeamScore++;
                if (set.FirstTeamScore >= game.PointsToWin &&
                    (set.FirstTeamScore - set.SecondTeamScore) >= game.PointDifferenceToWin)
                {
                    set.Winner = set.FirstTeam;
                    set.Status = GameStatus.Finished;
                    game.FirstTeamScore++;
                    var nextSet = game.Sets.FirstOrDefault(x => x.Number == set.Number + 1);
                    if (game.FirstTeamScore >= (game.MaxSets + 1) / 2)
                    {
                        game.Winner = game.FirstTeam;
                        game.Status = GameStatus.Finished;
                    }
                    else
                    {
                        nextSet.Status = GameStatus.Started;
                        nextSet.StartDate = DateTime.Now;
                        await _setRepository.UpdateAsync(nextSet);
                    }

                    await _gameRepository.UpdateAsync(game);
                    await _setRepository.UpdateAsync(set);
                }
            }
            else
            {
                set.SecondTeamScore++;
                if (set.SecondTeamScore >= game.PointsToWin &&
                    (set.SecondTeamScore - set.FirstTeamScore) >= game.PointDifferenceToWin)
                {
                    set.Winner = set.SecondTeam;
                    set.Status = GameStatus.Finished;
                    game.SecondTeamScore++;
                    var nextSet = game.Sets.FirstOrDefault(x => x.Number == set.Number + 1);
                    if (game.SecondTeamScore >= (game.MaxSets + 1) / 2)
                    {
                        game.Winner = game.SecondTeam;
                        game.Status = GameStatus.Finished;
                    }
                    else
                    {
                        nextSet.Status = GameStatus.Started;
                        await _setRepository.UpdateAsync(nextSet);
                    }

                    await _setRepository.UpdateAsync(set);
                    await _gameRepository.UpdateAsync(game);
                }
            }
        }
        else
        {
            if (player.Score > 0)
            {
                player.Score--;
                if (!player.Team)
                {
                    set.FirstTeamScore--;
                }
                else
                {
                    set.SecondTeamScore--;
                }
            }
            else
            {
                return BadRequest("Player's score cannot be below 0");
            }
        }

        int index = set.Players.IndexOf(player);
        if(index != -1)
            set.Players[index] = player;

        await _setRepository.UpdateAsync(set);

        return NoContent();
    }
}

