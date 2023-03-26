using Backend.Auth.Model;
using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Team;
using Backend.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeamRepository _teamRepository;

        public TeamController(IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, ITeamRepository teamRepository)
        {
            _authorizationService = authorizationService;
            _userManager = userManager;
            _teamRepository = teamRepository;
        }

        [Authorize(Roles = ApplicationUserRoles.Admin)]
        [HttpGet("/api/[controller]")]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamRepository.GetAllAsync();

            return Ok(teams);
        }

        [Authorize]
        [HttpGet("/api/User/{userId}/[controller]")]
        public async Task<IActionResult> GetUserTeams(string userId)
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

            var teams = await _teamRepository.GetAllAsync();
            var userTeams = teams.Where(x => x.OwnerId == user.Id).ToList();

            return Ok(userTeams);
        }
        
        [AllowAnonymous]
        [HttpGet("/api/[controller]/{teamId}")]
        public async Task<IActionResult> Get(Guid teamId)
        {
            var team = await _teamRepository.GetAsync(teamId);

            if (team == null)
            {
                return NotFound();
            }
            
            return Ok(team);
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
            
            var team = new Team
            {
                Title = addTeamDto.Title,
                PictureUrl = addTeamDto.PictureUrl,
                Description = addTeamDto.Description,
                CreateDate = DateTime.Now,
                LastEditDate = DateTime.Now,
                OwnerId = user.Id
            };

            var createdTeam = await _teamRepository.CreateAsync(team);
            
            return CreatedAtAction(nameof(Post), createdTeam.Id);
        }

        [Authorize]
        [HttpPut("/api/[controller]/{teamId}")]
        public async Task<IActionResult> Update(Guid teamId, [FromBody] EditTeamDto editTeamDto)
        {
            var team = await _teamRepository.GetAsync(teamId);

            if (team == null)
            {
                return NotFound();
            }
            
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

            team.Title = editTeamDto.Title ?? team.Title;
            team.PictureUrl = editTeamDto.PictureUrl ?? team.PictureUrl;
            team.Description = editTeamDto.Description ?? team.Description;

            await _teamRepository.UpdateAsync(team);
            
            return NoContent();
        }
        
        [Authorize]
        [HttpDelete("/api/[controller]/{teamId}")]
        public async Task<IActionResult> Delete(Guid teamId)
        {
            var team = await _teamRepository.GetAsync(teamId);

            if (team == null)
            {
                return NotFound();
            }
            
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

            await _teamRepository.DeleteAsync(teamId);

            return NoContent();
        }

        [Authorize]
        [HttpPatch("/api/[controller]/{teamId}/Player")]
        public async Task<IActionResult> AddPlayer(Guid teamId, [FromBody] AddTeamPlayerDto addTeamPlayerDto)
        {
            var team = await _teamRepository.GetAsync(teamId);

            if (team == null)
            {
                return NotFound();
            }
            
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

            if (team.Players.FirstOrDefault(x => x.Name == addTeamPlayerDto.Name) != null)
            {
                return BadRequest("Player with this name already exists");
            }

            var newTeamPlayer = new TeamPlayer
            {
                Name = addTeamPlayerDto.Name
            };

            team.Players.Add(newTeamPlayer);
            team.LastEditDate = DateTime.Now;

            await _teamRepository.UpdateAsync(team);
            
            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/[controller]/{teamId}/Player/{playerId}")]
        public async Task<IActionResult> RemovePlayers(Guid teamId, Guid playerId)
        {
            var team = await _teamRepository.GetAsync(teamId);

            if (team == null)
            {
                return NotFound();
            }
            
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
            
            var playerToRemove = team.Players.FirstOrDefault(x => x.Id == playerId);
            if (playerToRemove != null)
            {
                team.Players.Remove(playerToRemove);
                team.LastEditDate = DateTime.Now;
                await _teamRepository.UpdateAsync(team);
                return Ok();
            }
            else
            {
                BadRequest("Player doesn't exist");
            }

            return NoContent();
        }
    }
}
