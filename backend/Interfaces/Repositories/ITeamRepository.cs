using Backend.Data.Entities.Team;

namespace Backend.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task<IEnumerable<Team>> GetAllOwnedByUserAsync(string userId);
    Task<Team?> GetAsync(Guid teamId);
    Task<Team> CreateAsync(Team team);
    Task<Team> UpdateAsync(Team team);
    Task DeleteAsync(Guid teamId);
}