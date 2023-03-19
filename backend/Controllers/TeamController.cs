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
        private readonly ITeamRepository _teamRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public TeamController(ITeamRepository teamRepository, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            _teamRepository = teamRepository;
            _userManager = userManager;
            _authorizationService = authorizationService;
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
                return NotFound();
            }
            
            // Only if it's user owned resource or user is admin
            if (!User.IsInRole(ApplicationUserRoles.Admin))
            {
                if (user.Id != userId)
                {
                    return Forbid();
                }
            }

            var teams = await _teamRepository.GetAllOwnedByUserAsync(user.Id);

            return Ok(teams);
        }
        
        [AllowAnonymous]
        [HttpGet("/api/[controller]/{teamId}")]
        public async Task<IActionResult> Get(string teamId)
        {
            Guid teamIdGuid;
            try
            {
                teamIdGuid = new Guid(teamId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            var team = await _teamRepository.GetAsync(teamIdGuid);

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
                return NotFound();
            }
            
            var team = new Team();
            team.Title = addTeamDto.Title;
            team.PictureUrl = addTeamDto.PictureUrl;
            team.Description = addTeamDto.Description;
            team.CreationDate = DateTime.Now;
            team.LastEditDate = DateTime.Now;
            team.OwnerId = user.Id;

            var createdTeam = await _teamRepository.CreateAsync(team);
            
            return CreatedAtAction(nameof(Post), createdTeam.Id);
        }

        [Authorize]
        [HttpPut("/api/[controller]/{teamId}")]
        public async Task<IActionResult> Update(string teamId, [FromBody] EditTeamDto editTeamDto)
        {
            Guid teamIdGuid;
            try
            {
                teamIdGuid = new Guid(teamId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            var team = await _teamRepository.GetAsync(teamIdGuid);

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

            if (editTeamDto.Title != null)
            {
                team.Title = editTeamDto.Title;
            }
            
            if (editTeamDto.PictureUrl != null)
            {
                team.PictureUrl = editTeamDto.PictureUrl;
            }
            
            if (editTeamDto.Description != null)
            {
                team.Description = editTeamDto.Description;
            }
            
            var updatedTeam = await _teamRepository.UpdateAsync(team);
            
            return Ok(updatedTeam.Id);
        }
        
        [Authorize]
        [HttpDelete("/api/[controller]/{teamId}")]
        public async Task<IActionResult> Delete(string teamId)
        {
            Guid teamIdGuid;
            try
            {
                teamIdGuid = new Guid(teamId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            var team = await _teamRepository.GetAsync(teamIdGuid);

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

            await _teamRepository.DeleteAsync(teamIdGuid);

            return Ok();
        }

        [Authorize]
        [HttpPost("/api/[controller]/{teamId}/Player")]
        public async Task<IActionResult> AddPlayer(string teamId, [FromBody] AddTeamPlayerDto addTeamPlayerDto)
        {
            Guid teamIdGuid;
            try
            {
                teamIdGuid = new Guid(teamId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            var team = await _teamRepository.GetAsync(teamIdGuid);

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

            var newTeamPlayer = new TeamPlayer();
            newTeamPlayer.Name = addTeamPlayerDto.Name;

            team.Players.Add(newTeamPlayer);

            await _teamRepository.UpdateAsync(team);
            
            return Ok();
        }

        [Authorize]
        [HttpDelete("/api/[controller]/{teamId}/Player/{playerId}")]
        public async Task<IActionResult> RemovePlayers(string teamId, string playerId)
        {
            Guid teamIdGuid;
            try
            {
                teamIdGuid = new Guid(teamId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            Guid playerIdGuid;
            try
            {
                playerIdGuid = new Guid(playerId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            
            var team = await _teamRepository.GetAsync(teamIdGuid);

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
            
            var playerToRemove = team.Players.FirstOrDefault(x => x.Id == playerIdGuid);
            if (playerToRemove != null)
            {
                team.Players.Remove(playerToRemove);
                await _teamRepository.UpdateAsync(team);
                return Ok();
            }
            else
            {
                BadRequest("Player doesn't exist");
            }

            return Ok();
        }
    }
}
