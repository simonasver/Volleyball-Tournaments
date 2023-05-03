using Backend.Data.Entities.Tournament;

namespace Backend.Interfaces.Repositories;

public interface ITournamentMatchRepository
{
    Task<IList<TournamentMatch>> GetAllTournamentAsync(Guid tournamentId, bool allData);
    Task<TournamentMatch> UpdateAsync(TournamentMatch tournamentMatch);
}