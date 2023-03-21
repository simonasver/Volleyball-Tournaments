using Backend.Data.Entities.Game;

namespace Backend.Interfaces.Repositories;

public interface IGameRepository
{
    public Task<IEnumerable<Game>> GetAllAsync();
    public Task<Game?> GetAsync(Guid gameId);
    public Task<Game> CreateAsync(Game game);
    public Task<Game> UpdateAsync(Game game);
    public Task DeleteAsync(Game game);
}