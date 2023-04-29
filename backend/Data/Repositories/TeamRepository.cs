using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Extensions;
using Backend.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TeamRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _dbContext.Teams.ToListAsync();
    }
    public async Task<IEnumerable<Team>> GetAllAsync(SearchParameters searchParameters)
    {
        var queryable = _dbContext.Teams.AsQueryable().OrderByDescending(x => x.CreateDate).Include(x => x.Players);
        return await PagedList<Team>.CreateAsync(queryable, searchParameters.PageNumber, searchParameters.PageSize);
    }

    public async Task<IEnumerable<Team>> GetAllUserAsync(SearchParameters searchParameters, string userId)
    {
        var queryable = _dbContext.Teams.AsQueryable().OrderByDescending(x => x.CreateDate).Include(x => x.Players)
            .Where(x => x.OwnerId == userId);
        return await PagedList<Team>.CreateAsync(queryable, searchParameters.PageNumber, searchParameters.PageSize);
    }

    public async Task<Team?> GetAsync(Guid teamId)
    {
        return await _dbContext.Teams.Include(x => x.Players).OrderByDescending(x => x.CreateDate).ThenBy(x => x.Id).FirstOrDefaultAsync(x => x.Id == teamId);
    }

    public async Task<Team> CreateAsync(Team team)
    {
        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();
        return team;
    }

    public async Task<Team> UpdateAsync(Team team)
    {
        _dbContext.Teams.Update(team);
        await _dbContext.SaveChangesAsync();
        return team;
    }

    public async Task DeleteAsync(Guid teamId)
    {
        var teamToDelete = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
        if (teamToDelete != null)
        {
            _dbContext.Teams.Remove(teamToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}