using Backend.Data.Dtos.Tournament;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Services;

public interface ITournamentService
{
    public Task<ServiceResult<IEnumerable<Tournament>>> GetAllAsync(bool all, SearchParameters searchParameters);
    public Task<ServiceResult<IEnumerable<Tournament>>> GetUserTournamentsAsync(SearchParameters searchParameters, string userId);
    public Task<ServiceResult<Tournament>> GetAsync(Guid tournamentId);
    public Task<ServiceResult<IEnumerable<TournamentMatch>>> GetTournamentMatchesAsync(Guid tournamentId, bool allData);
    public Task<ServiceResult<Tournament>> CreateAsync(AddTournamentDto addTournamentDto, string userId);
    public Task<ServiceResult<bool>> UpdateAsync(Tournament tournament);
    public Task<ServiceResult<bool>> UpdateAsync(EditTournamentDto editTournamentDto, Tournament tournament);
    public Task<ServiceResult<bool>> UpdateMatchAsync(TournamentMatch match);
    public Task<ServiceResult<bool>> DeleteAsync(Tournament tournament);
    public Task<ServiceResult<bool>> TeamRequestJoinAsync(Tournament tournament, Team team);
    public Task<ServiceResult<bool>> AddTeamAsync(AddTeamToTournamentDto addTeamToTournamentDto, Tournament tournament);
    public Task<ServiceResult<bool>> RemoveTeamAsync(Tournament tournament, Guid teamId);
    public Task<ServiceResult<bool>> StartAsync(Tournament tournament);
    public Task<ServiceResult<bool>> MoveBracketAsync(Tournament tournament, Guid matchId);
    public Task<ServiceResult<bool>> GenerateAsync(int teamAmount, string userId);
}