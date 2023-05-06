using System.Text.Json;
using Backend.Data.Dtos.Tournament;
using Backend.Data.Entities.Auth;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Extensions;
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
    private readonly ITeamService _teamService;
    private readonly UserManager<ApplicationUser> _userManager;
    public TournamentsController(IAuthorizationService authorizationService, ITournamentService tournamentService, ITeamService teamService, UserManager<ApplicationUser> userManager)
    {
        _authorizationService = authorizationService;
        _tournamentService = tournamentService;
        _teamService = teamService;
        _userManager = userManager;
    }
    
    [AllowAnonymous]
    [HttpGet("/api/[controller]", Name = "GetTournaments")]
    public async Task<IActionResult> GetAll([FromQuery] bool all, [FromQuery] SearchParameters searchParameters)
    {
        if (all)
        {
            if (User.Identity == null || !User.IsInRole(ApplicationUserRoles.Admin))
            {
                return Forbid();
            }
        }
        
        var tournamentsResult = await _tournamentService.GetAllAsync(all, searchParameters);

        if (!tournamentsResult.IsSuccess)
        {
            return StatusCode(tournamentsResult.ErrorStatus, tournamentsResult.ErrorMessage);
        }

        var tournaments = (PagedList<Tournament>)tournamentsResult.Data!;
        
        var previousPageLink = tournaments.HasPrevious
            ? Url.Link("GetTournaments", new
            {
                pageNumber = searchParameters.PageNumber - 1,
                pageSize = searchParameters.PageSize
            })
            : null;

        var nextPageLink = tournaments.HasNext
            ? Url.Link("GetTournaments", new
            {
                pageNumber = searchParameters.PageNumber + 1,
                pageSize = searchParameters.PageSize
            })
            : null;
        
        var paginationMetadata = new
        {
            totalCount = tournaments.TotalCount,
            pageSize = tournaments.PageSize,
            currentPage = tournaments.CurrentPage,
            totalPages = tournaments.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
        
        return Ok(tournaments);
    }
    
    [Authorize]
    [HttpGet("/api/Users/{userId}/[controller]", Name = "GetUserTournaments")]
    public async Task<IActionResult> GetUserTournaments(string userId, [FromQuery] SearchParameters searchParameters)
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
        
        var userTournamentsResult = await _tournamentService.GetUserTournamentsAsync(searchParameters, user.Id);
        if (!userTournamentsResult.IsSuccess)
        {
            return StatusCode(userTournamentsResult.ErrorStatus, userTournamentsResult.ErrorMessage);
        }

        var userTournaments = (PagedList<Tournament>)userTournamentsResult.Data!;
        
        var previousPageLink = userTournaments.HasPrevious
            ? Url.Link("GetUserTournaments", new
            {
                pageNumber = searchParameters.PageNumber - 1,
                pageSize = searchParameters.PageSize
            })
            : null;

        var nextPageLink = userTournaments.HasNext
            ? Url.Link("GetUserTournaments", new
            {
                pageNumber = searchParameters.PageNumber + 1,
                pageSize = searchParameters.PageSize
            })
            : null;
        
        var paginationMetadata = new
        {
            totalCount = userTournaments.TotalCount,
            pageSize = userTournaments.PageSize,
            currentPage = userTournaments.CurrentPage,
            totalPages = userTournaments.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(userTournaments);
    }

    [AllowAnonymous]
    [HttpGet("/api/[controller]/{tournamentId}")]
    public async Task<IActionResult> Get(Guid tournamentId)
    {
        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

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

        var createdTournamentResult = await _tournamentService.CreateAsync(addTournamentDto, user.Id);

        if (!createdTournamentResult.IsSuccess)
        {
            return StatusCode(createdTournamentResult.ErrorStatus, createdTournamentResult.ErrorMessage);
        }
        
        return CreatedAtAction(nameof(Post), createdTournamentResult.Data!.Id);
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{tournamentId}")]
    public async Task<IActionResult> Patch(Guid tournamentId, [FromBody] EditTournamentDto editTournamentDto)
    {
        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

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

        var result = await _tournamentService.UpdateAsync(editTournamentDto, tournament);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("/api/[controller]/{tournamentId}")]
    public async Task<IActionResult> Delete(Guid tournamentId)
    {
        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

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

        var result = await _tournamentService.DeleteAsync(tournament);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return NoContent();
    }
    
    [Authorize]
    [HttpPost("/api/[controller]/{tournamentId}/RequestedTeams")]
    public async Task<IActionResult> RequestJoinTeam(Guid tournamentId, [FromBody] RequestJoinTournamentDto requestJoinTournamentDto)
    {
        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

        if (tournament == null)
        {
            return NotFound();
        }

        var teamResult = await _teamService.GetAsync(requestJoinTournamentDto.TeamId);

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

        var result = await _tournamentService.TeamRequestJoinAsync(tournament, team);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return Ok();
    }
    
    [Authorize]
    [HttpPost("/api/[controller]/{tournamentId}/AcceptedTeams")]
    public async Task<IActionResult> AddTeam(Guid tournamentId, [FromBody] AddTeamToTournamentDto addTeamToTournamentDto)
    {
        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

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

        var result = await _tournamentService.AddTeamAsync(addTeamToTournamentDto, tournament);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return Ok(tournament);
    }
    
    [Authorize]
    [HttpDelete("/api/[controller]/{tournamentId}/AcceptedTeams")]
    public async Task<IActionResult> RemoveTeam(Guid tournamentId, [FromQuery] Guid teamId)
    {
        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

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

        var result = await _tournamentService.RemoveTeamAsync(tournament, teamId);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result);
        }
        
        return NoContent();
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{tournamentId}/Status")]
    public async Task<IActionResult> Start(Guid tournamentId)
    {
        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

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

        var result = await _tournamentService.StartAsync(tournament);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpGet("/api/[controller]/{tournamentId}/Matches")]
    public async Task<IActionResult> GetTournamentMatches(Guid tournamentId)
    {
        var tournamentMatchesResult = await _tournamentService.GetTournamentMatchesAsync(tournamentId, false);

        if (!tournamentMatchesResult.IsSuccess)
        {
            return StatusCode(tournamentMatchesResult.ErrorStatus, tournamentMatchesResult.ErrorMessage);
        }

        var tournamentMatches = tournamentMatchesResult.Data;

        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

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

        return Ok(tournamentMatches);
    }

    [Authorize]
    [HttpPatch("/api/[controller]/{tournamentId}/Matches/{matchId}/Brackets")]
    public async Task<IActionResult> MoveBracket(Guid tournamentId, Guid matchId)
    {
        var tournamentResult = await _tournamentService.GetAsync(tournamentId);

        if (!tournamentResult.IsSuccess)
        {
            return StatusCode(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
        }

        var tournament = tournamentResult.Data;

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

        var result = await _tournamentService.MoveBracketAsync(tournament, matchId);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }

        return NoContent();
    }
    
    [Authorize(Roles = ApplicationUserRoles.Admin)]
    [HttpPost("/api/[controller]/generate")]
    public async Task<IActionResult> Generate([FromQuery] int teamAmount)
    {
        var user = await _userManager.FindByNameAsync(User.Identity?.Name);
        
        if (user == null)
        {
            return Forbid();
        }

        var result = await _tournamentService.GenerateAsync(teamAmount, user.Id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.ErrorStatus, result.ErrorMessage);
        }
        
        return Ok();
    }
}