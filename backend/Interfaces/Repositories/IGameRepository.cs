using Backend.Data.Entities.Game;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Repositories;

public interface IGameRepository
{
    public Task<IEnumerable<Game>> GetAllAsync(bool all, SearchParameters searchParameters);
    public Task<IEnumerable<Game>> GetAllUserAsync(SearchParameters searchParameters, string userId);
    public Task<Game?> GetAsync(Guid gameId);
    public Task<Game> CreateAsync(Game game);
    public Task<Game> UpdateAsync(Game game);
    public Task DeleteAsync(Guid gameId);
}