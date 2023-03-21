using Backend.Data.Entities.Game;
using Backend.Interfaces.Repositories;

namespace Backend.Data.Repositories;

public class GameRepository : IGameRepository
{
    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Game?> GetAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public async Task<Game> CreateAsync(Game game)
    {
        throw new NotImplementedException();
    }

    public async Task<Game> UpdateAsync(Game game)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Game game)
    {
        throw new NotImplementedException();
    }
}