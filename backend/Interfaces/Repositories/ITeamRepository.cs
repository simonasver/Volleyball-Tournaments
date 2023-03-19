using Backend.Data.Entities.Team;

namespace Backend.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task<IEnumerable<Team>> GetAllOwnedByUserAsync(string id);
    Task<Team?> GetAsync(Guid id);
    Task<Team> CreateAsync(Team team);
    Task<Team> UpdateAsync(Team team);
    Task DeleteAsync(Guid id);
}