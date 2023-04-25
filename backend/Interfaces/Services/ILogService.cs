using Backend.Data.Entities;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Services;

public interface ILogService
{
    public Task<ServiceResult<bool>> CreateLog(string message, bool isPrivate, string userId, Tournament? tournament = null, Game? game = null);
    public Task<ServiceResult<IEnumerable<Log>>> GetAllLogs();
    public Task<ServiceResult<IEnumerable<Log>>> GetTournamentLogs(Guid tournamentId);
    public Task<ServiceResult<IEnumerable<Log>>> GetGameLogs(Guid gameId);
}