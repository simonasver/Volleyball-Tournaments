using Backend.Data.Entities;

namespace Backend.Interfaces.Repositories;

public interface ILogRepository
{
    public Task<IEnumerable<Log>> GetAllGame(Guid gameId);
    public Task<Log> CreateAsync(Log log);
    public Task DeleteAllGameAsync(Guid gameId);
    public Task DeleteAllTournamentAsync(Guid tournamentId);
}