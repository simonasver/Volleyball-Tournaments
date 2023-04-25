using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentMatchRepository _tournamentMatchRepository;

    public TournamentService(ITournamentRepository tournamentRepository, ITournamentMatchRepository tournamentMatchRepository)
    {
        _tournamentRepository = tournamentRepository;
        _tournamentMatchRepository = tournamentMatchRepository;
    }
    
    public async Task<ServiceResult<IEnumerable<Tournament>>> GetAllAsync()
    {
        try
        {
            var tournaments = await _tournamentRepository.GetAllAsync();
            return ServiceResult<IEnumerable<Tournament>>.Success(tournaments);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Tournament>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<IEnumerable<Tournament>>> GetUserTournamentsAsync(string userId)
    {
        try
        {
            var tournaments = await _tournamentRepository.GetAllAsync();
            var userTournaments = tournaments.Where(x => x.OwnerId == userId).ToList();
            return ServiceResult<IEnumerable<Tournament>>.Success(userTournaments);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Tournament>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<Tournament>> GetAsync(Guid tournamentId)
    {
        Tournament tournament;
        try
        {
            tournament = await _tournamentRepository.GetAsync(tournamentId);
        }
        catch (Exception ex)
        {
            return ServiceResult<Tournament>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }

        if (tournament == null)
        {
            return ServiceResult<Tournament>.Failure(StatusCodes.Status404NotFound);
        }
        
        return ServiceResult<Tournament>.Success(tournament);
    }

    public async Task<ServiceResult<IEnumerable<TournamentMatch>>> GetTournamentMatchesAsync(Guid tournamentId, bool allData)
    {
        try
        {
            var matches = await _tournamentMatchRepository.GetAllTournamentAsync(tournamentId, allData);
            return ServiceResult<IEnumerable<TournamentMatch>>.Success(matches);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<TournamentMatch>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(Tournament tournament)
    {
        try
        {
            await _tournamentRepository.UpdateAsync(tournament);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateMatchAsync(TournamentMatch match)
    {
        try
        {
            await _tournamentMatchRepository.UpdateAsync(match);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}