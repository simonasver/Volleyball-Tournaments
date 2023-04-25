using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Services;

public interface ITournamentService
{
    public Task<ServiceResult<IEnumerable<Tournament>>> GetAllAsync();
    public Task<ServiceResult<IEnumerable<Tournament>>> GetUserTournamentsAsync(string userId);
    public Task<ServiceResult<Tournament>> GetAsync(Guid tournamentId);
    public Task<ServiceResult<IEnumerable<TournamentMatch>>> GetTournamentMatchesAsync(Guid tournamentId, bool allData);
    public Task<ServiceResult<bool>> UpdateAsync(Tournament tournament);
    public Task<ServiceResult<bool>> UpdateMatchAsync(TournamentMatch match);
}