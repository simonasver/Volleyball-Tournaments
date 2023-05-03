using Backend.Data.Dtos.Game;
using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Utils;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameTeamRepository _gameTeamRepository;
    private readonly ISetRepository _setRepository;
    private readonly ITournamentService _tournamentService;
    private readonly ILogService _logService;

    public GameService(IGameRepository gameRepository, IGameTeamRepository gameTeamRepository, ISetRepository setRepository, ITournamentService tournamentService, ILogService logService)
    {
        _gameRepository = gameRepository;
        _gameTeamRepository = gameTeamRepository;
        _setRepository = setRepository;
        _tournamentService = tournamentService;
        _logService = logService;
    }
    
    public async Task<ServiceResult<IEnumerable<Game>>> GetAllAsync(bool all, SearchParameters searchParameters)
    {
        try
        {
            var games = await _gameRepository.GetAllAsync(all, searchParameters);
            return ServiceResult<IEnumerable<Game>>.Success(games);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Game>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<IEnumerable<Game>>> GetUserGamesAsync(SearchParameters searchParameters, string userId)
    {
        try
        {
            var userGames = await _gameRepository.GetAllUserAsync(searchParameters, userId);
            return ServiceResult<IEnumerable<Game>>.Success(userGames);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Game>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<Game>> GetAsync(Guid gameId)
    {
        Game game;
        try
        {
            game = await _gameRepository.GetAsync(gameId);
        }
        catch (Exception ex)
        {
            return ServiceResult<Game>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }

        if (game == null)
        {
            return ServiceResult<Game>.Failure(StatusCodes.Status404NotFound);
        }
        
        return ServiceResult<Game>.Success(game);
    }

    public async Task<ServiceResult<Game>> CreateAsync(AddGameDto addGameDto, string userId)
    {
        if (addGameDto.MaxSets % 2 == 0)
        {
            return ServiceResult<Game>.Failure(StatusCodes.Status400BadRequest, "Max sets must be an odd number");
        }

        if (!String.IsNullOrEmpty(addGameDto.PictureUrl))
        {
            if (!(await Utils.IsLinkImage(addGameDto.PictureUrl)))
            {
                return ServiceResult<Game>.Failure(StatusCodes.Status400BadRequest, "Provided picture url was not an image");
            }
        }

        var newGame = new Game
        {
            Title = addGameDto.Title,
            PictureUrl = addGameDto.PictureUrl,
            Description = addGameDto.Description,
            Basic = addGameDto.Basic,
            PointsToWin = addGameDto.PointsToWin,
            PointsToWinLastSet = addGameDto.PointsToWinLastSet,
            PointDifferenceToWin = addGameDto.PointDifferenceToWin,
            MaxSets = addGameDto.MaxSets,
            PlayersPerTeam = addGameDto.PlayersPerTeam,
            IsPrivate = addGameDto.IsPrivate,
            OwnerId = userId,
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            FirstTeamScore = 0,
            SecondTeamScore = 0,
        };

        try
        {
            var createdGame = await _gameRepository.CreateAsync(newGame);
            return ServiceResult<Game>.Success(createdGame);
        }
        catch (Exception ex)
        {
            return ServiceResult<Game>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(EditGameDto editGameDto, Game game)
    {
        if (game.Status == GameStatus.Finished)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot edit finished game");
        }

        if (editGameDto.MaxSets % 2 == 0)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Max sets must be an odd number");
        }

        if (editGameDto.Title != null)
        {
            game.Title = editGameDto.Title;
        }
        
        if (!String.IsNullOrEmpty(editGameDto.PictureUrl))
        {
            if (!(await Utils.IsLinkImage(editGameDto.PictureUrl)))
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Provided picture url was not an image");
            }
            game.PictureUrl = editGameDto.PictureUrl;
        }

        if (editGameDto.Description != null)
        {
            game.Description = editGameDto.Description;
        }

        if (editGameDto.Basic != null)
        {
            game.Basic = editGameDto.Basic ?? false;
        }

        if (editGameDto.PointsToWin != null)
        {
            if (game.TournamentMatch != null)
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot edit tournament game points to win");
            }
            if (game.Status < GameStatus.Started)
            {
                game.PointsToWin = editGameDto.PointsToWin ?? game.PointsToWin;
            }
        }

        if (editGameDto.PointsToWinLastSet != null)
        {
            if (game.TournamentMatch != null)
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot edit tournament game points to win");
            }

            if (game.Status < GameStatus.Started)
            {
                game.PointsToWinLastSet = editGameDto.PointsToWinLastSet ?? game.PointsToWinLastSet;
            }
        }

        if (editGameDto.PointDifferenceToWin != null)
        {
            if (game.TournamentMatch != null)
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot edit tournament game points");
            }
            if (game.Status < GameStatus.Started)
            {
                game.PointDifferenceToWin = editGameDto.PointDifferenceToWin ?? game.PointDifferenceToWin;
            }
        }

        if (editGameDto.MaxSets != null)
        {
            if (game.Status < GameStatus.Started)
            {
                game.MaxSets = editGameDto.MaxSets ?? game.MaxSets;
            }
        }
        
        if (editGameDto.PlayersPerTeam != null)
        {
            if (game.Status == GameStatus.New)
            {
                game.PlayersPerTeam = editGameDto.PlayersPerTeam ?? game.PlayersPerTeam;
            }
        }

        if (editGameDto.IsPrivate != null)
        {
            game.IsPrivate = editGameDto.IsPrivate ?? game.IsPrivate;
        }
        
        game.LastEditDate = DateTime.Now;

        try
        {
            await _gameRepository.UpdateAsync(game);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Game game)
    {
        if (game.TournamentMatch != null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot delete tournament game without deleting the tournament");
        }
        
        try
        {
            var logsDeleteResult = await _logService.DeleteGameLogsAsync(game.Id);
            if (!logsDeleteResult.IsSuccess)
            {
                return ServiceResult<bool>.Failure(logsDeleteResult.ErrorStatus, logsDeleteResult.ErrorMessage);
            }
            await _gameRepository.DeleteAsync(game.Id);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> TeamRequestJoinAsync(Game game, Team team)
    {
        if (game.TournamentMatch != null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot join tournament game");
        }

        if (game.RequestedTeams.Any(x => x.Id == team.Id) || (game.FirstTeam != null && game.FirstTeam.Title == team.Title) ||
            (game.SecondTeam != null && game.SecondTeam.Title == team.Title))
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team already requested to join this game");
        }

        if (team.Players.Count != game.PlayersPerTeam && game.PlayersPerTeam != 0)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Teams in this game are required to have " + game.PlayersPerTeam + " players");
        }

        if (team.Players.Count == 0)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team must have at least 1 player");
        }

        game.RequestedTeams.Add(team);

        try
        {
            await _gameRepository.UpdateAsync(game);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<Game>> AddTeamAsync(AddTeamToGameDto addTeamToGameDto, Game game)
    {
        if (game.TournamentMatch != null)
        {
            return ServiceResult<Game>.Failure(StatusCodes.Status400BadRequest, "Cannot join tournament game");
        }

        if (!game.RequestedTeams.Any(x => x.Id == addTeamToGameDto.TeamId))
        {
            return ServiceResult<Game>.Failure(StatusCodes.Status400BadRequest, "Team has not requested to join the game");
        }

        var team = game.RequestedTeams.FirstOrDefault(x => x.Id == addTeamToGameDto.TeamId);
        
        if ((team.Players.Count != game.PlayersPerTeam && game.PlayersPerTeam != 0) || team.Players.Count == 0)
        {
            game.RequestedTeams.Remove(team);
            try
            {
                await _gameRepository.UpdateAsync(game);
                return ServiceResult<Game>.Failure(StatusCodes.Status400BadRequest, "Team has 0 or another incompatible number of players for this game");
            }
            catch (Exception ex)
            {
                return ServiceResult<Game>.Failure(StatusCodes.Status500InternalServerError);
            }
        }

        try
        {
            game = GameUtils.AddTeamToGame(game, team);
        }
        catch (Exception ex)
        {
            return ServiceResult<Game>.Failure(StatusCodes.Status400BadRequest, ex.Message);
        }
        
        game.LastEditDate = DateTime.Now;

        try
        {
            await _gameRepository.UpdateAsync(game);
            return ServiceResult<Game>.Success(game);
        }
        catch (Exception ex)
        {
            return ServiceResult<Game>.Failure(StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ServiceResult<bool>> RemoveTeamAsync(bool team, Game game)
    {
        if (game.TournamentMatch != null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot leave tournament game");
        }

        if (game.Status >= GameStatus.Started)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Game is already started");
        }

        if (team == false)
        {
            if (game.FirstTeam == null)
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Game does not have a first team");
            }

            try
            {
                await _gameTeamRepository.DeleteAsync(game.FirstTeam.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
            }
            game.FirstTeam = null;
        }
        else
        {
            if (game.SecondTeam == null)
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Game does not have a second team");
            }

            try
            {
                await _gameTeamRepository.DeleteAsync(game.SecondTeam.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
            }
            game.SecondTeam = null;
        }
        
        if ((game.FirstTeam != null && game.SecondTeam == null) || (game.FirstTeam == null && game.SecondTeam != null))
        {
            game.Status = GameStatus.SingleTeam;
        }
        else if (game.FirstTeam == null && game.SecondTeam == null)
        {
            game.Status = GameStatus.New;
        }
        
        game.LastEditDate = DateTime.Now;

        try
        {
            await _gameRepository.UpdateAsync(game);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> StartAsync(Game game)
    {
        if (game.Status == GameStatus.Started)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Game is already started");
        }

        if (game.Status == GameStatus.Finished)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Game is already finished");
        }

        if (game.Status != GameStatus.Ready)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Game does not two teams");
        }

        if (game.FirstTeam!.Players.Count != game.PlayersPerTeam && game.PlayersPerTeam != 0)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "First team has a different player count than needed (" + game.PlayersPerTeam + ")");
        }

        if (game.SecondTeam!.Players.Count != game.PlayersPerTeam && game.PlayersPerTeam != 0)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Second team has a different player count than needed (" + game.PlayersPerTeam + ")");
        }

        game.Status = GameStatus.Started;
        

        for (var i = 0; i < game.MaxSets; i++)
        {
            game = GameUtils.AddSetToGame(game, i);
        }

        var firstSet = game.Sets.FirstOrDefault(x => x.Number == 1)!;
        firstSet.StartDate = DateTime.Now;
        firstSet.Status = GameStatus.Started;

        game.LastEditDate = DateTime.Now;
        game.StartDate = DateTime.Now;

        try
        {
            await _gameRepository.UpdateAsync(game);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<IEnumerable<Set>>> GetGameSetsAsync(Guid gameId)
    {
        try
        {
            var allSets = await _setRepository.GetAllAsync();
            return ServiceResult<IEnumerable<Set>>.Success(allSets.Where(x => x.Game.Id == gameId).ToList());
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Set>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> ChangePlayerSetScoreAsync(ChangeSetPlayerScoreDto changeSetPlayerScoreDto, Game game, Guid setId, Guid playerId, string userId)
    {
        Tournament tournament = null;
        TournamentMatch tournamentMatch = null;
        ICollection<TournamentMatch> tournamentMatchesToUpdate = new List<TournamentMatch>();

        if (game.TournamentMatch != null)
        {
            var tournamentResult = await _tournamentService.GetAsync(game.TournamentMatch.Tournament.Id);
            if (!tournamentResult.IsSuccess)
            {
                return ServiceResult<bool>.Failure(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
            }

            tournament = tournamentResult.Data!;
            
            var tournamentMatchesResult = await _tournamentService.GetTournamentMatchesAsync(tournament.Id, true);
            if (!tournamentMatchesResult.IsSuccess)
            {
                return ServiceResult<bool>.Failure(tournamentResult.ErrorStatus, tournamentResult.ErrorMessage);
            }
            tournamentMatch = tournamentMatchesResult.Data.FirstOrDefault(x => x.Id == game.TournamentMatch.Id);
        }
        

        var set = game.Sets.FirstOrDefault(x => x.Id == setId);
        if (set == null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Set does not exist");
        }

        if (set.Status == GameStatus.Finished)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Set is already finished");
        }

        var player = set.Players.FirstOrDefault(x => x.Id == playerId);
        if (player == null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Player does not exist");
        }

        if (changeSetPlayerScoreDto.Change)
        {
            await _logService.CreateLogAsync("Player " + player.Name + " score was increased by 1", false, userId, game: game, tournament: game.TournamentMatch?.Tournament);
            player.Score++;
            if (!player.Team)
            {
                set.FirstTeamScore++;
                if (GameUtils.IsFinalSet(game, set) && (set.FirstTeamScore >= game.PointsToWinLastSet &&
                                                        (set.FirstTeamScore - set.SecondTeamScore) >= game.PointDifferenceToWin) || set.Number != game.MaxSets && (set.FirstTeamScore >= game.PointsToWin &&
                        (set.FirstTeamScore - set.SecondTeamScore) >= game.PointDifferenceToWin))
                {
                    await _logService.CreateLogAsync("First team won the " + set.Number + "set", false, userId, game: game, tournament: game.TournamentMatch?.Tournament);
                    set.Winner = set.FirstTeam;
                    set.Status = GameStatus.Finished;
                    game.FirstTeamScore++;
                    var nextSet = game.Sets.FirstOrDefault(x => x.Number == set.Number + 1);
                    if (game.FirstTeamScore >= (game.MaxSets + 1) / 2)
                    {
                        await _logService.CreateLogAsync("First team won the game", false, userId, game: game, tournament: game.TournamentMatch?.Tournament);
                        game.Winner = game.FirstTeam;
                        game.Status = GameStatus.Finished;
                        if (tournament != null)
                        {
                            try
                            {
                                (tournament, tournamentMatchesToUpdate) =
                                    TournamentUtils.MatchesToUpdateInTournamentAfterWonMatch(tournament,
                                        tournamentMatch);
                            }
                            catch (Exception ex)
                            {
                                return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
                            }
                        }
                    }
                    else
                    {
                        nextSet.Status = GameStatus.Started;
                        nextSet.StartDate = DateTime.Now;
                        try
                        {
                            await _setRepository.UpdateAsync(nextSet);
                        }
                        catch (Exception ex)
                        {
                            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
                        }
                    }

                    try
                    {
                        await _gameRepository.UpdateAsync(game);
                        await _setRepository.UpdateAsync(set);
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
                    }
                }
            }
            else
            {
                set.SecondTeamScore++;
                if (GameUtils.IsFinalSet(game, set) && (set.SecondTeamScore >= game.PointsToWinLastSet &&
                    (set.SecondTeamScore - set.FirstTeamScore) >= game.PointDifferenceToWin) || set.Number != game.MaxSets && ((set.SecondTeamScore >= game.PointsToWin &&
                        (set.SecondTeamScore - set.FirstTeamScore) >= game.PointDifferenceToWin)))
                {
                    await _logService.CreateLogAsync("Second team won the " + set.Number + "set", false, userId, game: game, tournament: game.TournamentMatch?.Tournament);
                    set.Winner = set.SecondTeam;
                    set.Status = GameStatus.Finished;
                    game.SecondTeamScore++;
                    var nextSet = game.Sets.FirstOrDefault(x => x.Number == set.Number + 1);
                    if (game.SecondTeamScore >= (game.MaxSets + 1) / 2)
                    {
                        await _logService.CreateLogAsync("Second team won the game", false, userId, game: game, tournament: game.TournamentMatch?.Tournament);
                        game.Winner = game.SecondTeam;
                        game.Status = GameStatus.Finished;
                        if (tournament != null)
                        {
                            try
                            {
                                (tournament, tournamentMatchesToUpdate) =
                                    TournamentUtils.MatchesToUpdateInTournamentAfterWonMatch(tournament,
                                        tournamentMatch);
                            }
                            catch (Exception ex)
                            {
                                ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
                            }
                        }
                    }
                    else
                    {
                        nextSet.Status = GameStatus.Started;
                        try
                        {
                            await _setRepository.UpdateAsync(nextSet);
                        }
                        catch (Exception ex)
                        {
                            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
                        }
                    }

                    try
                    {
                        await _setRepository.UpdateAsync(set);
                        await _gameRepository.UpdateAsync(game);
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
                    }
                }
            }
        }
        else
        {
            if (player.Score > 0)
            {
                await _logService.CreateLogAsync("Player " + player.Name + " score was decreased by 1", false, userId, game: game, tournament: game.TournamentMatch?.Tournament);
                player.Score--;
                if (!player.Team)
                {
                    set.FirstTeamScore--;
                }
                else
                {
                    set.SecondTeamScore--;
                }
            }
            else
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Player's score cannot be below 0");
            }
        }

        int index = set.Players.IndexOf(player);
        if(index != -1)
            set.Players[index] = player;
        
        game.LastEditDate = DateTime.Now;

        if (tournament != null)
        {
            var result = await _tournamentService.UpdateAsync(tournament);
            if (!result.IsSuccess)
            {
                return ServiceResult<bool>.Failure(result.ErrorStatus, result.ErrorMessage);
            }
            foreach (var matchToUpdate in tournamentMatchesToUpdate)
            {
                result = await _tournamentService.UpdateMatchAsync(matchToUpdate);
                if (!result.IsSuccess)
                {
                    return ServiceResult<bool>.Failure(result.ErrorStatus, result.ErrorMessage);
                }
                foreach (var gameTeam in new [] {matchToUpdate.Game.FirstTeam, matchToUpdate.Game.SecondTeam})
                {
                    if (gameTeam != null)
                    {
                        try
                        {
                            await _gameTeamRepository.UpdateAsync(gameTeam);
                        }
                        catch (Exception ex)
                        {
                            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
                        }
                    }
                }
            }
        }

        try
        {
            await _setRepository.UpdateAsync(set);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> ChangePlayerSetStatsAsync(ChangeSetPlayerStatsDto changeSetPlayerStatsDto,
        Game game,
        Guid setId, Guid playerId, string userId)
    {
        if (game.Basic)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "This game does not have explicit scoreboard");
        }
        
        var set = game.Sets.FirstOrDefault(x => x.Id == setId);
        if (set == null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Set does not exist");
        }

        if (set.Status == GameStatus.Finished)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Set is already finished");
        }

        var player = set.Players.FirstOrDefault(x => x.Id == playerId);
        if (player == null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Player does not exist");
        }

        async Task CreateStatChangeLog(string stat)
        {
            await _logService.CreateLogAsync("Player " + player.Name + " " + stat + " were " + (changeSetPlayerStatsDto.Change ? "increased" : "decreased") + " by one", false, userId, game: game, tournament: game.TournamentMatch?.Tournament);
        }

        string lessThanZeroMessage = "Cannot reduce to more than zero!";

        switch (changeSetPlayerStatsDto.Type)
        {
            case SetPlayerStats.Kills:
                if (player.Kills == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }

                await CreateStatChangeLog("kills");
                player.Kills = (uint)((player.Kills ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Errors:
                if (player.Errors == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("errors");
                player.Errors = (uint)((player.Errors ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Attempts:
                if (player.Attempts == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("attempts");
                player.Attempts = (uint)((player.Attempts ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.SuccessfulBlocks:
                if (player.SuccessfulBlocks == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("successful blocks");
                player.SuccessfulBlocks = (uint)((player.SuccessfulBlocks ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Blocks:
                if (player.Blocks == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("blocks");
                player.Blocks = (uint)((player.Blocks ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Touches:
                if (player.Touches == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("touches");
                player.Touches = (uint)((player.Touches ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.BlockingErrors:
                if (player.BlockingErrors == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("blocking errors");
                player.BlockingErrors = (uint)((player.BlockingErrors ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.Aces:
                if (player.Aces == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("aces");
                player.Aces = (uint)((player.Aces ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.ServingErrors:
                if (player.ServingErrors == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("serving errors");
                player.ServingErrors = (uint)((player.ServingErrors ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.TotalServes:
                if (player.TotalServes == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("total serves");
                player.TotalServes = (uint)((player.TotalServes ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.SuccessfulDigs:
                if (player.SuccessfulDigs == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("successful digs");
                player.SuccessfulDigs = (uint)((player.SuccessfulDigs ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.BallTouches:
                if (player.BallTouches == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("ball touches");
                player.BallTouches = (uint)((player.BallTouches ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
            case SetPlayerStats.BallMisses:
                if (player.BallMisses == 0 && !changeSetPlayerStatsDto.Change)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, lessThanZeroMessage);
                }
                await CreateStatChangeLog("ball misses");
                player.BallMisses = (uint)((player.BallMisses ?? 0) + (changeSetPlayerStatsDto.Change ? 1 : -1));
                break;
        }

        try
        {
            await _setRepository.UpdateAsync(set);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}