using System.Security.Policy;
using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Utils;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;

    public TeamService(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }
    
    public async Task<ServiceResult<IEnumerable<Team>>> GetAllAsync(SearchParameters searchParameters)
    {
        try
        {
            var teams = await _teamRepository.GetAllAsync(searchParameters);
            return ServiceResult<IEnumerable<Team>>.Success(teams);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Team>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    public async Task<ServiceResult<IEnumerable<Team>>> GetUserTeamsAsync(SearchParameters searchParameters, string userId)
    {
        IEnumerable<Team> teams;
        try
        {
            teams = await _teamRepository.GetAllUserAsync(searchParameters, userId);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Team>>.Failure(StatusCodes.Status500InternalServerError, "Not found");
        }
        
        return ServiceResult<IEnumerable<Team>>.Success(teams);
    }

    public async Task<ServiceResult<Team>> GetAsync(Guid teamId)
    {
        Team team;
        try
        {
            team = await _teamRepository.GetAsync(teamId);
        }
        catch (Exception ex)
        {
            return ServiceResult<Team>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
        if (team == null)
        {
            return ServiceResult<Team>.Failure(StatusCodes.Status404NotFound);
        }

        return ServiceResult<Team>.Success(team);
    }

    public async Task<ServiceResult<Team>> CreateAsync(AddTeamDto addTeamDto, string userId)
    {
        var teams = await _teamRepository.GetAllAsync();

        if (teams.Any(x => x.Title == addTeamDto.Title))
        {
            return ServiceResult<Team>.Failure(StatusCodes.Status400BadRequest,"Team title must be unique");
        }

        if (!String.IsNullOrEmpty(addTeamDto.PictureUrl))
        {
            if (!(await Utils.IsLinkImage(addTeamDto.PictureUrl)))
            {
                return ServiceResult<Team>.Failure(StatusCodes.Status400BadRequest,"Provided picture url was not an image");
            }
        }
        
        var team = new Team
        {
            Title = addTeamDto.Title,
            PictureUrl = addTeamDto.PictureUrl,
            Description = addTeamDto.Description,
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            OwnerId = userId
        };

        try
        {
            var createdTeam = await _teamRepository.CreateAsync(team);
            return ServiceResult<Team>.Success(createdTeam);
        }
        catch (Exception ex)
        {
            return ServiceResult<Team>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(EditTeamDto editTeamDto, Team team)
    {
        var teams = await _teamRepository.GetAllAsync();

        if (teams.Any(x => x.Title == editTeamDto.Title))
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team title must be unique");
        }

        if (editTeamDto.Title != null)
        {
            team.Title = editTeamDto.Title;
        }

        if (!String.IsNullOrEmpty(editTeamDto.PictureUrl))
        {
            if (!(await Utils.IsLinkImage(editTeamDto.PictureUrl)))
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Provided picture url was not an image");
            }
            team.PictureUrl = editTeamDto.PictureUrl;
        }

        if (editTeamDto.Description != null)
        {
            team.Description = editTeamDto.Description;
        }

        try
        {
            await _teamRepository.UpdateAsync(team);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid teamId)
    {
        try
        {
            await _teamRepository.DeleteAsync(teamId);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> AddPlayerAsync(AddTeamPlayerDto addTeamPlayerDto, Team team)
    {
        if (team.Players.Count >= 12)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team is already full (max number players is 12)");
        }

        if (team.Players.FirstOrDefault(x => x.Name == addTeamPlayerDto.Name) != null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Player with this name already exists");
        }

        var newTeamPlayer = new TeamPlayer
        {
            Name = addTeamPlayerDto.Name
        };

        team.Players.Add(newTeamPlayer);
        team.LastEditDate = DateTime.Now;

        try
        {
            await _teamRepository.UpdateAsync(team);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> RemovePlayerAsync(Guid playerId, Team team)
    {
        var playerToRemove = team.Players.FirstOrDefault(x => x.Id == playerId);
        if (playerToRemove != null)
        {
            team.Players.Remove(playerToRemove);
            team.LastEditDate = DateTime.Now;
            try
            {
                await _teamRepository.UpdateAsync(team);
                return ServiceResult<bool>.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Player doesn't exist");
    }
}