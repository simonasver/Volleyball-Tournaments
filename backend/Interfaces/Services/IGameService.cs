﻿using Backend.Data.Dtos.Game;
using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Auth;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;

namespace Backend.Interfaces.Services;

public interface IGameService
{
    public Task<ServiceResult<IEnumerable<Game>>> GetAllAsync(bool all, SearchParameters searchParameters);
    public Task<ServiceResult<IEnumerable<Game>>> GetUserGamesAsync(SearchParameters searchParameters, string userId);
    public Task<ServiceResult<Game>> GetAsync(Guid gameId);
    public Task<ServiceResult<Game>> CreateAsync(AddGameDto addGameDto, string userId);
    public Task<ServiceResult<bool>> UpdateAsync(EditGameDto editGameDto, Game game);
    public Task<ServiceResult<bool>> DeleteAsync(Game game);
    public Task<ServiceResult<bool>> TeamRequestJoinAsync(Game game, Team team);
    public Task<ServiceResult<Game>> AddTeamAsync(AddTeamToGameDto addTeamToGameDto, Game game);
    public Task<ServiceResult<bool>> RemoveTeamAsync(bool team, Game game);
    public Task<ServiceResult<bool>> StartAsync(Game game);
    public Task<ServiceResult<IEnumerable<Set>>> GetGameSetsAsync(Guid gameId);
    public Task<ServiceResult<bool>> ChangePlayerSetScoreAsync(ChangeSetPlayerScoreDto changeSetPlayerScoreDto, Game game, Guid setId, Guid playerId, string userId);

    public Task<ServiceResult<bool>> ChangePlayerSetStatsAsync(ChangeSetPlayerStatsDto changeSetPlayerStatsDto, Game game,
        Guid setId, Guid playerId, string userId);

    public Task<ServiceResult<bool>> AddManagerAsync(Game game, ApplicationUser user);
    public Task<ServiceResult<bool>> RemoveManagerAsync(Game game, ApplicationUser user);
}