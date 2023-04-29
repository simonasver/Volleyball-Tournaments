using System.Security.Claims;
using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Services;

public interface ITeamService
{
    public Task<ServiceResult<IEnumerable<Team>>> GetAllAsync(SearchParameters searchParameters);
    public Task<ServiceResult<IEnumerable<Team>>> GetUserTeamsAsync(SearchParameters searchParameters, string userId);
    public Task<ServiceResult<Team>> GetAsync(Guid teamId);
    public Task<ServiceResult<Team>> CreateAsync(AddTeamDto addTeamDto, string userId);
    public Task<ServiceResult<bool>> UpdateAsync(EditTeamDto editTeamDto, Team team);
    public Task<ServiceResult<bool>> DeleteAsync(Guid teamId);
    public Task<ServiceResult<bool>> AddPlayerAsync(AddTeamPlayerDto addTeamPlayerDto, Team team);
    public Task<ServiceResult<bool>> RemovePlayerAsync(Guid playerId, Team team);
}