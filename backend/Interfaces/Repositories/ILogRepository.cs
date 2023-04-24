using Backend.Data.Entities;

namespace Backend.Interfaces.Repositories;

public interface ILogRepository
{
    public Task<IEnumerable<Log>> GetAllAsync();
    public Task<IEnumerable<Log>> GetAllTournament(Guid tournamentId);
    public Task<IEnumerable<Log>> GetAllGame(Guid gameId);
    public Task<Log?> GetAsync(Guid logId);
    public Task<Log> CreateAsync(Log log);
    public Task<Log> UpdateAsync(Log log);
    public Task DeleteAsync(Guid logId);
}