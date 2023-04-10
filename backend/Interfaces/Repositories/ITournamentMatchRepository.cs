using Backend.Data.Entities.Tournament;

namespace Backend.Interfaces.Repositories;

public interface ITournamentMatchRepository
{
    Task<IEnumerable<TournamentMatch>> GetAllAsync();
    Task<IEnumerable<TournamentMatch>> GetAllTournamentAsync(Guid tournamentId);
    Task<TournamentMatch?> GetAsync(Guid tournamentMatchId);
    Task<TournamentMatch> CreateAsync(TournamentMatch tournamentMatch);
    Task<TournamentMatch> UpdateAsync(TournamentMatch tournamentMatch);
    Task DeleteAsync(Guid tournamentMatchId);
}