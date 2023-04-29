using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync();
    Task<IEnumerable<Team>> GetAllAsync(SearchParameters searchParameters);
    Task<IEnumerable<Team>> GetAllUserAsync(SearchParameters searchParameters, string userId);
    Task<Team?> GetAsync(Guid teamId);
    Task<Team> CreateAsync(Team team);
    Task<Team> UpdateAsync(Team team);
    Task DeleteAsync(Guid teamId);
}