using Backend.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;
        private readonly ISetRepository _setRepository;
        
        public GameController(IGameRepository gameRepository, ISetRepository setRepository)
        {
            _gameRepository = gameRepository;
            _setRepository = setRepository;
        }
        
        [AllowAnonymous]
        [HttpGet("/api/[controller]")]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameRepository.GetAllAsync();

            return Ok(games);
        }
    }
}

