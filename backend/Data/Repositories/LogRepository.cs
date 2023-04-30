using Backend.Data.Entities;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class LogRepository : ILogRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LogRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Log>> GetAllAsync()
    {
        return await _dbContext.Logs.ToListAsync();
    }

    public async Task<IEnumerable<Log>> GetAllTournament(Guid tournamentId)
    {
        return await _dbContext.Logs.Where(x => x.Tournament != null && x.Tournament.Id == tournamentId).OrderByDescending(x => x.Time).ToListAsync();
    }

    public async Task<IEnumerable<Log>> GetAllGame(Guid gameId)
    {
        return await _dbContext.Logs.Where(x => x.Game != null && x.Game.Id == gameId).OrderByDescending(x => x.Time).ToListAsync();;
    }

    public async Task<Log?> GetAsync(Guid logId)
    {
        return await _dbContext.Logs.FirstOrDefaultAsync(x => x.Id == logId);
    }

    public async Task<Log> CreateAsync(Log log)
    {
        _dbContext.Logs.Add(log);
        await _dbContext.SaveChangesAsync();
        return log;
    }

    public async Task<Log> UpdateAsync(Log log)
    {
        _dbContext.Logs.Update(log);
        await _dbContext.SaveChangesAsync();
        return log;
    }

    public async Task DeleteAsync(Guid logId)
    {
        var logToDelete = await _dbContext.Logs.FirstOrDefaultAsync(x => x.Id == logId);
        if (logToDelete != null)
        {
            _dbContext.Logs.Remove(logToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAllGameAsync(Guid gameId)
    {
        _dbContext.Logs.RemoveRange(_dbContext.Logs.Where(x => x.Game.Id == gameId));
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAllTournamentAsync(Guid tournamentId)
    {
        _dbContext.Logs.RemoveRange(_dbContext.Logs.Where(x => x.Tournament.Id == tournamentId));
        await _dbContext.SaveChangesAsync();
    }
}