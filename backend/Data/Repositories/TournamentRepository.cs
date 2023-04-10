using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
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
    
    
    public async Task<IEnumerable<Tournament>> GetAllAsync()
    {
        return await _dbContext.Tournaments.ToListAsync();
    }

    public async Task<Tournament?> GetAsync(Guid tournamentId)
    {
        return await _dbContext.Tournaments
            .Include(x => x.RequestedTeams)
                .ThenInclude(x => x.Players)
            .Include(x => x.AcceptedTeams)
            .Include(x => x.Matches)
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