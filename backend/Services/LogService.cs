using Backend.Data.Entities.Game;
using Backend.Data.Entities.Log;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;
    
    public LogService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<ServiceResult<bool>> CreateLogAsync(string message, bool isPrivate, string userId, Tournament? tournament = null, Game? game = null)
    {
        if (string.IsNullOrEmpty(message))
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Message cannot be null");
        }

        try
        {
            await _logRepository.CreateAsync(new Log()
            {
                IsPrivate = isPrivate,
                OwnerId = userId,
                Message = message,
                Tournament = tournament,
                Game = game,
                Time = DateTime.Now
            });
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    public async Task<ServiceResult<IEnumerable<Log>>> GetGameLogsAsync(Guid gameId)
    {
        try
        {
            var logs = await _logRepository.GetAllGame(gameId);
            return ServiceResult<IEnumerable<Log>>.Success(logs);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Log>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> DeleteGameLogsAsync(Guid gameId)
    {
        try
        {
            await _logRepository.DeleteAllGameAsync(gameId);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> DeleteTournamentLogsAsync(Guid tournamentId)
    {
        try
        {
            await _logRepository.DeleteAllTournamentAsync(tournamentId);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}