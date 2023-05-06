using Backend.Data.Entities.Game;
using Backend.Data.Entities.Log;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Services;

public interface ILogService
{
    public Task<ServiceResult<bool>> CreateLogAsync(string message, bool isPrivate, string userId, Tournament? tournament = null, Game? game = null);
    public Task<ServiceResult<IEnumerable<Log>>> GetGameLogsAsync(Guid gameId);
    public Task<ServiceResult<bool>> DeleteGameLogsAsync(Guid gameId);
    public Task<ServiceResult<bool>> DeleteTournamentLogsAsync(Guid tournamentId);
}