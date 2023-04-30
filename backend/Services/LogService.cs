using Backend.Data.Entities;
using Backend.Data.Entities.Game;
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

    public async Task<ServiceResult<IEnumerable<Log>>> GetAllLogsAsync()
    {
        return ServiceResult<IEnumerable<Log>>.Success(await _logRepository.GetAllAsync());
    }

    public async Task<ServiceResult<IEnumerable<Log>>> GetTournamentLogsAsync(Guid tournamentId)
    {
        return ServiceResult<IEnumerable<Log>>.Success(await _logRepository.GetAllTournament(tournamentId));
    }

    public async Task<ServiceResult<IEnumerable<Log>>> GetGameLogsAsync(Guid gameId)
    {
        return ServiceResult<IEnumerable<Log>>.Success(await _logRepository.GetAllGame(gameId));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Log log)
    {
        try
        {
            await _logRepository.DeleteAsync(log.Id);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
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