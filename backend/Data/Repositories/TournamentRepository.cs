using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Extensions;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TournamentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public async Task<IEnumerable<Tournament>> GetAllAsync(bool all, SearchParameters searchParameters)
    {
        var queryable = _dbContext.Tournaments.AsQueryable();
        if (!all)
        {
            queryable = queryable.Where(x => !x.IsPrivate);
        }

        queryable = queryable.Where(x => x.Title.Contains(searchParameters.SearchInput)).OrderByDescending(x => x.CreateDate).Include(x => x.AcceptedTeams).Include(x => x.Winner);
        return await PagedList<Tournament>.CreateAsync(queryable, searchParameters.PageNumber, searchParameters.PageSize);
    }

    public async Task<IEnumerable<Tournament>> GetAllUserAsync(SearchParameters searchParameters, string userId)
    {
        var queryable = _dbContext.Tournaments.Where(x => x.Title.Contains(searchParameters.SearchInput)).AsQueryable().OrderByDescending(x => x.CreateDate).Where(x => x.OwnerId == userId).Include(x => x.AcceptedTeams).Include(x => x.Winner);
        return await PagedList<Tournament>.CreateAsync(queryable, searchParameters.PageNumber, searchParameters.PageSize);
    }

    public async Task<Tournament?> GetAsync(Guid tournamentId)
    {
        return await _dbContext.Tournaments
            .Include(x => x.RequestedTeams)
                .ThenInclude(x => x.Players)
            .Include(x => x.AcceptedTeams)
                .ThenInclude(x => x.Players)
            .Include(x => x.Winner)
            .Include(x => x.Managers)
            .FirstOrDefaultAsync(x => x.Id == tournamentId);
    }

    public async Task<Tournament> CreateAsync(Tournament tournament)
    {
        _dbContext.Tournaments.Add(tournament);
        await _dbContext.SaveChangesAsync();
        return tournament;
    }

    public async Task<Tournament> UpdateAsync(Tournament tournament)
    {
        _dbContext.Tournaments.Update(tournament);
        await _dbContext.SaveChangesAsync();
        return tournament;
    }

    public async Task DeleteAsync(Guid tournamentId)
    {
        var tournamentToDelete = await _dbContext.Tournaments.FirstOrDefaultAsync(x => x.Id == tournamentId);
        if (tournamentToDelete != null)
        {
            _dbContext.Tournaments.Remove(tournamentToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}