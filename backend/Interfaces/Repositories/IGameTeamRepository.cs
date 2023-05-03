using Backend.Data.Entities.Game;

namespace Backend.Interfaces.Repositories;

public interface IGameTeamRepository
{
    Task<GameTeam> UpdateAsync(GameTeam gameTeam);
    Task DeleteAsync(Guid gameTeamId);
}