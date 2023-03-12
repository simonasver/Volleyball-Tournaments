using Backend.Data.Entities.Team;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TeamRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<IEnumerable<Team>> GetAll()
    {
        return await _dbContext.Teams.ToListAsync();
    }

    public async Task<Team?> Get(string id)
    {
        var guidId = new Guid(id);
        return await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == guidId);
    }

    public async Task<Team> Create(Team team)
    {
        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();
        return team;
    }

    public async Task<Team> Put(Team team)
    {
        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();
        return team;
    }

    public async Task Delete(string id)
    {
        var guidId = new Guid(id);
        var teamToDelete = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == guidId);
        if (teamToDelete != null)
        {
            _dbContext.Teams.Remove(teamToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}