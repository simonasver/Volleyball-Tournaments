using Backend.Data.Entities.Tournament;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class TournamentMatchRepository : ITournamentMatchRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public TournamentMatchRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<IEnumerable<TournamentMatch>> GetAllAsync()
    {
        return await _dbContext.TournamentMatches.ToListAsync();
    }

    public async Task<IList<TournamentMatch>> GetAllTournamentAsync(Guid tournamentId, bool allData)
    {
        if (allData)
        {
            return await _dbContext.TournamentMatches
            .Include(x => x.Tournament)
            .Include(x => x.Parents)
            .Include(x => x.Child)
            .Include(x => x.Game)
                .ThenInclude(x => x.FirstTeam)
                    .ThenInclude(x => x.Players)
            .Include(x => x.Game)
                .ThenInclude(x => x.SecondTeam)
                    .ThenInclude(x => x.Players)
            .Where(x => x.Tournament.Id == tournamentId)
            .ToListAsync();
        }
        else
        {
            return await _dbContext.TournamentMatches
                .Include(x => x.Tournament)
                .Include(x => x.Parents)
                .Include(x => x.Child)
                .Include(x => x.Game)
                    .ThenInclude(x => x.FirstTeam)
                .Include(x => x.Game)
                    .ThenInclude(x => x.SecondTeam)
                .Where(x => x.Tournament.Id == tournamentId)
                .ToListAsync();
        }
    }

    public async Task<TournamentMatch?> GetAsync(Guid tournamentMatchId)
    {
        return await _dbContext.TournamentMatches
            .Include(x => x.Tournament)
            .Include(x => x.Parents)
            .Include(x => x.Child)
            .Include(x => x.Game)
            .ThenInclude(x => x.FirstTeam)
            .ThenInclude(x => x.Players)
            .Include(x => x.Game)
            .ThenInclude(x => x.SecondTeam)
            .ThenInclude(x => x.Players)
            .FirstOrDefaultAsync(x => x.Id == tournamentMatchId);
    }

    public async Task<TournamentMatch> CreateAsync(TournamentMatch tournamentMatch)
    {
        _dbContext.TournamentMatches.Add(tournamentMatch);
        await _dbContext.SaveChangesAsync();
        return tournamentMatch;
    }

    public async Task<TournamentMatch> UpdateAsync(TournamentMatch tournamentMatch)
    {
        _dbContext.TournamentMatches.Update(tournamentMatch);
        await _dbContext.SaveChangesAsync();
        return tournamentMatch;
    }

    public async Task DeleteAsync(Guid tournamentMatchId)
    {
        var tournamentMatchToDelete = await _dbContext.TournamentMatches.FirstOrDefaultAsync(x => x.Id == tournamentMatchId);
        if (tournamentMatchToDelete != null)
        {
            _dbContext.TournamentMatches.Remove(tournamentMatchToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}