using Backend.Data.Entities.Team;

namespace Backend.Interfaces.Repositories;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAll();
    Task<Team?> Get(string id);
    Task<Team> Create(Team team);
    Task<Team> Put(Team team);
    Task Delete(string id);
}