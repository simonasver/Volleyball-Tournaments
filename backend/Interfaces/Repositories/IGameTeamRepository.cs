using Backend.Data.Entities.Game;

namespace Backend.Interfaces.Repositories;

public interface IGameTeamRepository
{
    Task<IEnumerable<GameTeam>> GetAllAsync();
    Task<GameTeam?> GetAsync(Guid gameTeamId);
    Task<GameTeam> CreateAsync(GameTeam gameTeam);
    Task<GameTeam> UpdateAsync(GameTeam gameTeam);
    Task DeleteAsync(Guid gameTeamId);
}