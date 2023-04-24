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
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentMatchRepository _tournamentMatchRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ISetRepository _setRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IGameTeamRepository _gameTeamRepository;
    private readonly ILogService _logService;

    public GamesController(ITournamentService tournamentService, IGameService gameService, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, ITournamentRepository tournamentRepository, ITournamentMatchRepository tournamentMatchRepository, IGameRepository gameRepository, ISetRepository setRepository, ITeamRepository teamRepository, IGameTeamRepository gameTeamRepository, ILogService logService)
    {
        _tournamentService = tournamentService;
        _gameService = gameService;
        _authorizationService = authorizationService;
        _userManager = userManager;
        _tournamentRepository = tournamentRepository;
        _tournamentMatchRepository = tournamentMatchRepository;
        _gameRepository = gameRepository;
        _setRepository = setRepository;
        _teamRepository = teamRepository;
        _gameTeamRepository = gameTeamRepository;
        _logService = logService;
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
            games = (await _gameRepository.GetAllAsync()).Where(x => !x.IsPrivate && x.TournamentMatch == null).ToList(); 
        }

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
        var userGames = games.Where(x => x.OwnerId == user.Id && x.TournamentMatch == null).ToList();

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

        if (!String.IsNullOrEmpty(addGameDto.PictureUrl))
        {
            if (!(await Utils.IsLinkImage(addGameDto.PictureUrl)))
            {
                return BadRequest("Provided picture url was not an image");
            }
        }

        var newGame = new Game
        {
            Title = addGameDto.Title,
            PictureUrl = addGameDto.PictureUrl,
            Description = addGameDto.Description,
            Basic = addGameDto.Basic,
            PointsToWin = addGameDto.PointsToWin,
            PointsToWinLastSet = addGameDto.PointsToWinLastSet,
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
    [HttpPatch("/api/[controller]/{gameId}")]
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
        
        if (!String.IsNullOrEmpty(editGameDto.PictureUrl))
        {
            if (!(await Utils.IsLinkImage(editGameDto.PictureUrl)))
            {
                return BadRequest("Provided picture url was not an image");
            }
            game.PictureUrl = editGameDto.PictureUrl;
        }

        if (editGameDto.Description != null)
        {
            game.Description = editGameDto.Description;
        }

        if (editGameDto.Basic != null)
        {
            game.Basic = editGameDto.Basic ?? false;
        }

        if (editGameDto.PointsToWin != null)
        {
            if (game.TournamentMatch != null)
            {
                return BadRequest("Cannot edit tournament game points to win");
            }
            if (game.Status < GameStatus.Started)
            {
                game.PointsToWin = editGameDto.PointsToWin ?? game.PointsToWin;
            }
        }

        if (editGameDto.PointsToWinLastSet != null)
        {
            if (game.TournamentMatch != null)
            {
                return BadRequest("Cannot edit tournament game points to win");
            }

            if (game.Status < GameStatus.Started)
            {
                game.PointsToWinLastSet = editGameDto.PointsToWinLastSet ?? game.PointsToWinLastSet;
            }
        }

        if (editGameDto.PointDifferenceToWin != null)
        {
            if (game.TournamentMatch != null)
            {
                return BadRequest("Cannot edit tournament game points");
            }
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

        if (game.TournamentMatch != null)
        {
            return BadRequest("Cannot delete tournament game without deleting the tournament");
        }
        
        await _gameRepository.DeleteAsync(gameId);

        return NoContent();
    }

    [Authorize]
    [HttpPost("/api/[controller]/{gameId}/RequestedTeams")]
    public async Task<IActionResult> RequestJoinTeam(Guid gameId, [FromBody] RequestJoinGameDto requestJoinGameDto)
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

        if (game.TournamentMatch != null)
        {
            return BadRequest("Cannot join tournament game");
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

        if (game.TournamentMatch != null)
        {
            return BadRequest("Cannot join tournament game");
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
        
        game.LastEditDate = DateTime.Now;
        
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

        if (game.TournamentMatch != null)
        {
            return BadRequest("Cannot leave tournament game");
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

            await _gameTeamRepository.DeleteAsync(game.FirstTeam.Id);
            game.FirstTeam = null;
        }
        else
        {
            if (game.SecondTeam == null)
            {
                return BadRequest("Game does not have a second team");
            }

            await _gameTeamRepository.DeleteAsync(game.SecondTeam.Id);
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
        
        game.LastEditDate = DateTime.Now;
        
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

        var sets = (await _setRepository.GetAllAsync()).Where(x => x.Game.Id == gameId).ToList();

        return Ok(sets);
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}/Sets/{setId}/Players/{playerId}/Score")]
    public async Task<IActionResult> ChangeSetPlayerScore(Guid gameId, Guid setId, Guid playerId, [FromBody] ChangeSetPlayerScoreDto changeSetPlayerScoreDto)
    {
        var game = await _gameRepository.GetAsync(gameId);
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

        Tournament tournament = null;
        TournamentMatch tournamentMatch = null;
        ICollection<TournamentMatch> tournamentMatchesToUpdate = new List<TournamentMatch>();

        if (game.TournamentMatch != null)
        {
            tournament = await _tournamentRepository.GetAsync(game.TournamentMatch.Tournament.Id);
            tournamentMatch = (await _tournamentMatchRepository.GetAllTournamentAsync(tournament.Id, true)).FirstOrDefault(x => x.Id == game.TournamentMatch.Id);
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
            await _logService.CreateLog("Player " + player.Name + " score was increased by 1", false, user.Id, game: game, tournament: game.TournamentMatch?.Tournament);
            player.Score++;
            if (!player.Team)
            {
                set.FirstTeamScore++;
                if (GameUtils.IsFinalSet(game, set) && (set.FirstTeamScore >= game.PointsToWinLastSet &&
                                                        (set.FirstTeamScore - set.SecondTeamScore) >= game.PointDifferenceToWin) || set.Number != game.MaxSets && (set.FirstTeamScore >= game.PointsToWin &&
                        (set.FirstTeamScore - set.SecondTeamScore) >= game.PointDifferenceToWin))
                {
                    await _logService.CreateLog("First team won the " + set.Number + "set", false, user.Id, game: game, tournament: game.TournamentMatch?.Tournament);
                    set.Winner = set.FirstTeam;
                    set.Status = GameStatus.Finished;
                    game.FirstTeamScore++;
                    var nextSet = game.Sets.FirstOrDefault(x => x.Number == set.Number + 1);
                    if (game.FirstTeamScore >= (game.MaxSets + 1) / 2)
                    {
                        await _logService.CreateLog("First team won the game", false, user.Id, game: game, tournament: game.TournamentMatch?.Tournament);
                        game.Winner = game.FirstTeam;
                        game.Status = GameStatus.Finished;
                        if (tournament != null)
                        {
                            (tournament, tournamentMatchesToUpdate) =
                                _tournamentService.MatchesToUpdateInTournamentAfterWonMatch(tournament, tournamentMatch);
                        }
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
                if (GameUtils.IsFinalSet(game, set) && (set.SecondTeamScore >= game.PointsToWinLastSet &&
                    (set.SecondTeamScore - set.FirstTeamScore) >= game.PointDifferenceToWin) || set.Number != game.MaxSets && ((set.SecondTeamScore >= game.PointsToWin &&
                        (set.SecondTeamScore - set.FirstTeamScore) >= game.PointDifferenceToWin)))
                {
                    await _logService.CreateLog("Second team won the " + set.Number + "set", false, user.Id, game: game, tournament: game.TournamentMatch?.Tournament);
                    set.Winner = set.SecondTeam;
                    set.Status = GameStatus.Finished;
                    game.SecondTeamScore++;
                    var nextSet = game.Sets.FirstOrDefault(x => x.Number == set.Number + 1);
                    if (game.SecondTeamScore >= (game.MaxSets + 1) / 2)
                    {
                        await _logService.CreateLog("Second team won the game", false, user.Id, game: game, tournament: game.TournamentMatch?.Tournament);
                        game.Winner = game.SecondTeam;
                        game.Status = GameStatus.Finished;
                        if (tournament != null)
                        {
                            (tournament, tournamentMatchesToUpdate) =
                                _tournamentService.MatchesToUpdateInTournamentAfterWonMatch(tournament, tournamentMatch);
                        }
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
                await _logService.CreateLog("Player " + player.Name + " score was decreased by 1", false, user.Id, game: game, tournament: game.TournamentMatch?.Tournament);
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
        
        game.LastEditDate = DateTime.Now;

        if (tournament != null)
        {
            await _tournamentRepository.UpdateAsync(tournament);
            foreach (var matchToUpdate in tournamentMatchesToUpdate)
            {
                await _tournamentMatchRepository.UpdateAsync(matchToUpdate);
                foreach (var gameTeam in new [] {matchToUpdate.Game.FirstTeam, matchToUpdate.Game.SecondTeam})
                {
                    if(gameTeam != null)
                        await _gameTeamRepository.UpdateAsync(gameTeam);
                }
            }
        }

        await _setRepository.UpdateAsync(set);

        return NoContent();
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{gameId}/Sets/{setId}/Players/{playerId}/Stats")]
    public async Task<IActionResult> ChangeSetPlayerStats(Guid gameId, Guid setId, Guid playerId,
        [FromBody] ChangeSetPlayerStatsDto changeSetPlayerStatsDto)
    {
        var game = await _gameRepository.GetAsync(gameId);
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

        if (game.Basic)
        {
            return BadRequest("This game does not have explicit scoreboard");
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

        async Task CreateStatChangeLog(string stat)
        {
            await _logService.CreateLog("Player " + player.Name + " " + stat + " were " + (changeSetPlayerStatsDto.Change ? "increased" : "decreased") + " by one", false, user.Id, game: game, tournament: game.TournamentMatch?.Tournament);
        }

        string lessThanZeroMessage = "Cannot reduce to more than zero!";

        switch (changeSetPlayerStatsDto.Type)
        {
            case SetPlayerStats.Kills:
                if (player.Kills == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }

                await CreateStatChangeLog("kills");
                player.Kills = (uint)((player.Kills ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Errors:
                if (player.Errors == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("errors");
                player.Errors = (uint)((player.Errors ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Attempts:
                if (player.Attempts == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("attempts");
                player.Attempts = (uint)((player.Attempts ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.SuccessfulBlocks:
                if (player.SuccessfulBlocks == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("successful blocks");
                player.SuccessfulBlocks = (uint)((player.SuccessfulBlocks ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Blocks:
                if (player.Blocks == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("blocks");
                player.Blocks = (uint)((player.Blocks ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Touches:
                if (player.Touches == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("touches");
                player.Touches = (uint)((player.Touches ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.BlockingErrors:
                if (player.BlockingErrors == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("blocking errors");
                player.BlockingErrors = (uint)((player.BlockingErrors ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Aces:
                if (player.Aces == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("aces");
                player.Aces = (uint)((player.Aces ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.ServingErrors:
                if (player.ServingErrors == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("serving errors");
                player.ServingErrors = (uint)((player.ServingErrors ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.TotalServes:
                if (player.TotalServes == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("total serves");
                player.TotalServes = (uint)((player.TotalServes ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.SuccessfulDigs:
                if (player.SuccessfulDigs == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("successful digs");
                player.SuccessfulDigs = (uint)((player.SuccessfulDigs ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.BallTouches:
                if (player.BallTouches == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("ball touches");
                player.BallTouches = (uint)((player.BallTouches ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.BallMisses:
                if (player.BallMisses == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return BadRequest(lessThanZeroMessage);
                }
                await CreateStatChangeLog("ball misses");
                player.BallMisses = (uint)((player.BallMisses ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
        }

        await _setRepository.UpdateAsync(set);
        
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("/api/[controller]/{gameId}/Logs")]
    public async Task<IActionResult> GetLogs(Guid gameId)
    {
        var game = await _gameRepository.GetAsync(gameId);
        if (game == null)
        {
            return BadRequest("Game does not exist");
        }
        
        var logs = await _logService.GetGameLogs(gameId);

        if (!logs.IsSuccess)
        {
            return StatusCode(logs.ErrorStatus ?? 500, logs.ErrorMessage);
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

