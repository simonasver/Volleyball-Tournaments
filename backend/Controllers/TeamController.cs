using Backend.Auth.Model;
using Backend.Data.Dtos.Team;
using Backend.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _teamRepository;

        public TeamController(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        [Authorize(Roles = ApplicationUserRoles.Admin)]
        [HttpGet("/api/[controller]")]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamRepository.GetAll();
            return Ok(teams);
        }
        
        [AllowAnonymous]
        [HttpGet("/api/[controller]/{teamId}")]
        public async Task<IActionResult> Get(string teamId)
        {
            var team = await _teamRepository.Get(teamId);
            return Ok(team);
        }

        // [Authorize]
        // [HttpPost("/api/[controller]")]
        // public async Task<IActionResult> Post([FromBody] AddTeamDto addTeamDto)
        // {
        //     
        // }
    }
}
