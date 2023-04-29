using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Repositories;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<IEnumerable<Tournament>> GetAllAsync(bool all, SearchParameters searchParameters);
    Task<IEnumerable<Tournament>> GetAllUserAsync(SearchParameters searchParameters, string userId);
    Task<Tournament?> GetAsync(Guid tournamentId);
    Task<Tournament> CreateAsync(Tournament tournament);
    Task<Tournament> UpdateAsync(Tournament tournament);
    Task DeleteAsync(Guid tournamentId);
}