using Backend.Data.Entities.Tournament;

namespace Backend.Interfaces.Repositories;

public interface ITournamentMatchRepository
{
    Task<IEnumerable<TournamentMatch>> GetAllAsync();
    Task<IList<TournamentMatch>> GetAllTournamentAsync(Guid tournamentId, bool allData);
    Task<TournamentMatch?> GetAsync(Guid tournamentMatchId);
    Task<TournamentMatch> CreateAsync(TournamentMatch tournamentMatch);
    Task<TournamentMatch> UpdateAsync(TournamentMatch tournamentMatch);
    Task DeleteAsync(Guid tournamentMatchId);
}