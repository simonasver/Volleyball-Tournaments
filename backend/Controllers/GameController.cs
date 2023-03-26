using Backend.Auth.Model;
using Backend.Data.Dtos.Game;
using Backend.Data.Entities.Game;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGameRepository _gameRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ISetRepository _setRepository;
        
        public GameController(IGameService gameService, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IGameRepository gameRepository, ITeamRepository teamRepository, ISetRepository setRepository)
        {
            _gameService = gameService;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _gameRepository = gameRepository;
            _teamRepository = teamRepository;
            _setRepository = setRepository;
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
        [HttpGet("/api/User/{userId}/[controller]")]
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
            };

            var createdGame = await _gameRepository.CreateAsync(newGame);
            
            return CreatedAtAction(nameof(Post), createdGame);
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

            if (editGameDto.MaxSets % 2 == 0)
            {
                return BadRequest("Max sets must be an odd number");
            }

            game.Title = editGameDto.Title ?? game.Title;
            game.Description = editGameDto.Description ?? game.Description;
            game.PointsToWin = editGameDto.PointsToWin ?? game.PointsToWin;
            game.PointDifferenceToWin = editGameDto.PointDifferenceToWin ?? game.PointDifferenceToWin;
            game.MaxSets = editGameDto.MaxSets ?? game.MaxSets;
            game.IsPrivate = editGameDto.IsPrivate ?? game.IsPrivate;
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
        [HttpPost("/api/[controller]/{gameId}/RequestedTeam")]
        public async Task<IActionResult> RequestTeam(Guid gameId, [FromBody] Guid teamId)
        {
            var game = await _gameRepository.GetAsync(gameId);

            if (game == null)
            {
                return NotFound();
            }

            var team = await _teamRepository.GetAsync(teamId);

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

            if (team.Players.Count != game.PlayersPerTeam)
            {
                return BadRequest("Teams in this game are required to have " + game.PlayersPerTeam + " players");
            }
            
            game.RequestedTeams.Add(team);

            return Ok();
        }

        [Authorize]
        [HttpPost("/api/[controller]/{gameId}/GameTeam")]
        public async Task<IActionResult> AddTeam(Guid gameId, [FromBody] Guid teamId)
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

            if (!game.RequestedTeams.Any(x => x.Id == teamId))
            {
                return BadRequest();
            }
            
            game = _gameService.AddTeamToGame(game, game.RequestedTeams.FirstOrDefault(x => x.Id == teamId));
            
            return Ok(game);
        }

        [Authorize]
        [HttpDelete("/api/[controller]/{gameId}/GameTeam/{teamId}")]
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

            if (game.Status != GameStatus.New)
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

            if (game.FirstTeam == null || game.SecondTeam == null)
            {
                return BadRequest("Game does not two teams");
            }

            game.Status = GameStatus.Started;
            

            for (var i = 0; i < game.MaxSets; i++)
            {
                game = _gameService.AddSetToGame(game);
            }

            game.LastEditDate = DateTime.Now;
            game.StartDate = DateTime.Now;

            await _gameRepository.UpdateAsync(game);

            return Ok();
        }
    }
}

