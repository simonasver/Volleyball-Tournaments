using Backend.Auth.Model;
using Backend.Data.Dtos.Game;
using Backend.Data.Dtos.Tournament;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Helpers.Extensions;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ITournamentService _tournamentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentMatchRepository _tournamentMatchRepository;
    private readonly ITeamRepository _teamRepository;
    public TournamentsController(IAuthorizationService authorizationService, ITournamentService tournamentService, UserManager<ApplicationUser> userManager, ITournamentRepository tournamentRepository, ITournamentMatchRepository tournamentMatchRepository, ITeamRepository teamRepository)
    {
        _authorizationService = authorizationService;
        _tournamentService = tournamentService;
        _userManager = userManager;
        _tournamentRepository = tournamentRepository;
        _tournamentMatchRepository = tournamentMatchRepository;
        _teamRepository = teamRepository;
    }
    
    [AllowAnonymous]
    [HttpGet("/api/[controller]")]
    public async Task<IActionResult> GetAll([FromQuery] bool all)
    {
        IEnumerable<Tournament> tournaments;
        if (all)
        {
            if (User.Identity == null || !User.IsInRole(ApplicationUserRoles.Admin))
            {
                return Forbid();
            }

            tournaments = await _tournamentRepository.GetAllAsync();
        }
        else
        {
            tournaments = (await _tournamentRepository.GetAllAsync()).Where(x => !x.IsPrivate).ToList(); 
        }

        return Ok(tournaments);
    }
    
    [Authorize]
    [HttpGet("/api/Users/{userId}/[controller]")]
    public async Task<IActionResult> GetUserTournaments(string userId)
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

        var tournaments = await _tournamentRepository.GetAllAsync();
        var userTournaments = tournaments.Where(x => x.OwnerId == user.Id).ToList();

        return Ok(userTournaments);
    }

    [AllowAnonymous]
    [HttpGet("/api/[controller]/{tournamentId}")]
    public async Task<IActionResult> Get(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetAsync(tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }

        if (tournament.IsPrivate)
        {
            // Only if it's user owned resource or user is admin
            if (!User.IsInRole(ApplicationUserRoles.Admin))
            {
                var authorization =
                    await _authorizationService.AuthorizeAsync(User, tournament, PolicyNames.ResourceOwner);
                if (!authorization.Succeeded)
                {
                    return NotFound();
                }
            }
        }

        return Ok(tournament);
    }

    [Authorize]
    [HttpPost("/api/[controller]")]
    public async Task<IActionResult> Post([FromBody] AddTournamentDto addTournamentDto)
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
        
        if (addTournamentDto.MaxSets % 2 == 0)
        {
            return BadRequest("Max sets must be an odd number");
        }

        var newTournament = new Tournament()
        {
            Title = addTournamentDto.Title,
            PictureUrl = addTournamentDto.PictureUrl,
            Description = addTournamentDto.Description,
            Type = addTournamentDto.Type,
            MaxTeams = addTournamentDto.MaxTeams,
            PointsToWin = addTournamentDto.PointsToWin,
            PointDifferenceToWin = addTournamentDto.PointDifferenceToWin,
            MaxSets = addTournamentDto.MaxSets,
            PlayersPerTeam = addTournamentDto.PlayersPerTeam,
            IsPrivate = addTournamentDto.IsPrivate,
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            Status = TournamentStatus.Open,
            OwnerId = user.Id
        };

        var createdTournament = await _tournamentRepository.CreateAsync(newTournament);
        
        return CreatedAtAction(nameof(Post), createdTournament.Id);
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{tournamentId}")]
    public async Task<IActionResult> Patch(Guid tournamentId, [FromBody] EditTournamentDto editTournamentDto)
    {
        var tournament = await _tournamentRepository.GetAsync(tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, tournament, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        if (tournament.Status == TournamentStatus.Finished)
        {
            return BadRequest("Cannot edit finished tournament");
        }

        if (editTournamentDto.MaxSets % 2 == 0)
        {
            return BadRequest("Max sets must be an odd number");
        }

        if (editTournamentDto.Title != null)
        {
            tournament.Title = editTournamentDto.Title;
        }

        if (editTournamentDto.PictureUrl != null)
        {
            tournament.PictureUrl = editTournamentDto.PictureUrl;
        }

        if (editTournamentDto.Description != null)
        {
            tournament.Description = editTournamentDto.Description;
        }

        if (editTournamentDto.Type != null)
        {
            tournament.Type = editTournamentDto.Type ?? tournament.Type;
        }

        if (editTournamentDto.MaxTeams != null)
        {
            tournament.MaxTeams = editTournamentDto.MaxTeams ?? tournament.MaxTeams;
        }

        if (editTournamentDto.PointsToWin != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.PointsToWin = editTournamentDto.PointsToWin ?? tournament.PointsToWin;
            }
        }

        if (editTournamentDto.PointDifferenceToWin != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.PointDifferenceToWin = editTournamentDto.PointDifferenceToWin ?? tournament.PointDifferenceToWin;
            }
        }

        if (editTournamentDto.MaxSets != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.MaxSets = editTournamentDto.MaxSets ?? tournament.MaxSets;
            }
        }
        
        if (editTournamentDto.PlayersPerTeam != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.PlayersPerTeam = editTournamentDto.PlayersPerTeam ?? tournament.PlayersPerTeam;
            }
        }

        if (editTournamentDto.IsPrivate != null)
        {
            tournament.IsPrivate = editTournamentDto.IsPrivate ?? tournament.IsPrivate;
        }
        
        tournament.LastEditDate = DateTime.Now;

        await _tournamentRepository.UpdateAsync(tournament);
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("/api/[controller]/{tournamentId}")]
    public async Task<IActionResult> Delete(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetAsync(tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, tournament, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        await _tournamentRepository.DeleteAsync(tournamentId);

        return NoContent();
    }
    
    [Authorize]
    [HttpPost("/api/[controller]/{tournamentId}/RequestedTeams")]
    public async Task<IActionResult> RequestJoinTeam(Guid tournamentId, [FromBody] RequestJoinTournamentDto requestJoinTournamentDto)
    {
        var tournament = await _tournamentRepository.GetAsync(tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }

        var team = await _teamRepository.GetAsync(requestJoinTournamentDto.TeamId);

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

        if (tournament.RequestedTeams.Any(x => x.Id == team.Id) || tournament.AcceptedTeams.Any(x => x.Title == team.Title))
        {
            return BadRequest("Team already requested to join this game");
        }

        if (team.Players.Count != tournament.PlayersPerTeam && tournament.PlayersPerTeam != 0)
        {
            return BadRequest("Teams in this tournament are required to have " + tournament.PlayersPerTeam + " players");
        }

        if (team.Players.Count == 0)
        {
            return BadRequest("Team must have at least 1 player");
        }

        tournament.RequestedTeams.Add(team);

        await _tournamentRepository.UpdateAsync(tournament);

        return Ok();
    }
    
    [Authorize]
    [HttpPost("/api/[controller]/{tournamentId}/AcceptedTeams")]
    public async Task<IActionResult> AddTeam(Guid tournamentId, [FromBody] AddTeamToTournamentDto addTeamToTournamentDto)
    {
        var tournament = await _tournamentRepository.GetAsync(tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, tournament, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        if (tournament.AcceptedTeams.Count >= tournament.MaxTeams)
        {
            return BadRequest("Tournament is already full");
        }

        if (!tournament.RequestedTeams.Any(x => x.Id == addTeamToTournamentDto.TeamId))
        {
            return BadRequest("Team has not requested to join the game");
        }

        var team = tournament.RequestedTeams.FirstOrDefault(x => x.Id == addTeamToTournamentDto.TeamId);
        
        if ((team.Players.Count != tournament.PlayersPerTeam && tournament.PlayersPerTeam != 0) || team.Players.Count == 0)
        {
            tournament.RequestedTeams.Remove(team);
            await _tournamentRepository.UpdateAsync(tournament);
            return BadRequest("Team has 0 or another incompatible number of players for this game");
        }

        tournament = _tournamentService.AddTeamToTournament(tournament, team);

        await _tournamentRepository.UpdateAsync(tournament);
        
        return Ok(tournament);
    }
    
    [Authorize]
    [HttpDelete("/api/[controller]/{tournamentId}/AcceptedTeams")]
    public async Task<IActionResult> RemoveTeam(Guid tournamentId, [FromQuery] Guid teamId)
    {
        var tournament = await _tournamentRepository.GetAsync(tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, tournament, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        if (tournament.Status >= TournamentStatus.Started)
        {
            return BadRequest("Game is already started");
        }

        var team = tournament.AcceptedTeams.FirstOrDefault(x => x.Id == teamId);

        if (team != null)
        {
            tournament.AcceptedTeams.Remove(team);
        }

        await _tournamentRepository.UpdateAsync(tournament);
        
        return NoContent();
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{tournamentId}/Status")]
    public async Task<IActionResult> Start(Guid tournamentId)
    {
        var tournament = await _tournamentRepository.GetAsync(tournamentId);

        if (tournament == null)
        {
            return NotFound();
        }
        
        // Only if it's user owned resource or user is admin
        if (!User.IsInRole(ApplicationUserRoles.Admin))
        {
            var authorization =
                await _authorizationService.AuthorizeAsync(User, tournament, PolicyNames.ResourceOwner);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }
        }

        if (tournament.Status == TournamentStatus.Started)
        {
            return BadRequest("Tournament is already started");
        }

        if (tournament.Status == TournamentStatus.Finished)
        {
            return BadRequest(("Tournament is already finished"));
        }

        if (tournament.AcceptedTeams.Count < 2)
        {
            return BadRequest("Tournament does not two teams");
        }

        tournament.Status = TournamentStatus.Started;

        var roundCount = tournament.AcceptedTeams.CountTournamentRounds();
        var generatedMatches = _tournamentService.GenerateEmptyBracket(tournament, roundCount).ToList();

        foreach (var tournamentMatch in generatedMatches)
        {
            tournament.Matches.Add(tournamentMatch);
        }

        await _tournamentRepository.UpdateAsync(tournament);

        return Ok();
    }
    
    [Authorize(Roles = ApplicationUserRoles.Admin)]
    [HttpPost("/api/[controller]/generate")]
    public async Task<IActionResult> Generate([FromQuery] int teamAmount)
    {
        if (teamAmount == null || teamAmount < 0 || teamAmount > 128)
        {
            return BadRequest("Team amount must be between 1 and 128");
        }
        
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        if (user == null)
        {
            return Forbid();
        }

        var generatedNamePrefix = DateTime.Now.ToShortTimeString();

        var tournament = new Tournament()
        {
            Title = "Tournament" + generatedNamePrefix,
            Description = "Generated" + generatedNamePrefix + " very good tournament",
            Type = TournamentType.SingleElimination,
            MaxTeams = 128,
            IsPrivate = false,
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            Status = TournamentStatus.Open,
            RequestedTeams = new List<Team>(),
            AcceptedTeams = new List<GameTeam>(),
            PointsToWin = 25,
            PointDifferenceToWin = 2,
            MaxSets = 5,
            PlayersPerTeam = 6,
            OwnerId = user.Id
        };

        for (int i = 0; i < teamAmount; i++)
        {
            var team = new GameTeam()
            {
                Title = "Team" + generatedNamePrefix + (i+1),
                Description = "Generated" + generatedNamePrefix + " very good team " + (i+1),
                Players = new List<GameTeamPlayer>(),
            };
            for (int j = 0; j < 6; j++)
            {
                var player = new GameTeamPlayer()
                {
                    Name = "Player" + (j+1)
                };
                team.Players.Add(player);
            }
            tournament.AcceptedTeams.Add(team);
        }

        await _tournamentRepository.CreateAsync(tournament);
        return Ok();
    }
}