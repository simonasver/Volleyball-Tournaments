using Backend.Data.Dtos.Game;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace TestProject.Services;

public class GameServiceTests
{
    private Mock<IGameRepository> _gameRepository;
    private Mock<IGameTeamRepository> _gameTeamRepository;
    private Mock<ISetRepository> _setRepository;
    private Mock<ITournamentService> _tournamentService;
    private Mock<ILogService> _logService;
    private IGameService _gameService;

    private IList<Game> _games;
    private IList<Team> _teams;
    private IList<Tournament> _tournaments;
    
    private Guid _fakeGuid;
    
    [SetUp]
    public void Setup()
    {
        _gameRepository = new Mock<IGameRepository>();
        _gameTeamRepository = new Mock<IGameTeamRepository>();
        _setRepository = new Mock<ISetRepository>();
        _tournamentService = new Mock<ITournamentService>();
        _logService = new Mock<ILogService>();
        _gameService = new GameService(_gameRepository.Object, _gameTeamRepository.Object, _setRepository.Object, _tournamentService.Object,
            _logService.Object);
        
        _fakeGuid = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        _games = Helpers.GetTestGameData();
        _teams = Helpers.GetTestTeamData();
        _tournaments = Helpers.GetTestTournamentData();
    }

    [Test]
    public async Task GetAllAsync_ReturnsAll()
    {
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        _gameRepository.Setup(x => x.GetAllAsync(true, searchParameters)).ReturnsAsync(_games);

        var result = await _gameService.GetAllAsync(true, searchParameters);
        
        Assert.AreEqual(_games.Count, result.Data.Count());
    }

    [Test]
    public async Task GetUserGamesAsync_ReturnsUserGames()
    {
        var data = new List<Game> { _games[0], _games[1] };
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        _gameRepository.Setup(x => x.GetAllUserAsync(searchParameters, "first")).ReturnsAsync(data);

        var result = await _gameService.GetUserGamesAsync(searchParameters, "first");
        
        Assert.AreEqual(data.Count, result.Data.Count());
    }

    [Test]
    public async Task GetAsync_ReturnsGame()
    {
        _gameRepository.Setup(x => x.GetAsync(_games[0].Id)).ReturnsAsync(_games[0]);

        var result = await _gameService.GetAsync(_games[0].Id);
        
        Assert.AreEqual(_games[0].Id, result.Data.Id);
    }

    [Test]
    public async Task GetAsyncWithWrongId_Returns404()
    {
        _gameRepository.Setup(x => x.GetAsync(_fakeGuid)).ReturnsAsync((Game)null);

        var result = await _gameService.GetAsync(_fakeGuid);
        
        Assert.AreEqual(StatusCodes.Status404NotFound, result.ErrorStatus);
    }

    [Test]
    public async Task CreateAsync_ReturnsGame()
    {
        _gameRepository.Setup(x => x.CreateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.CreateAsync(
            new AddGameDto()
            {
                Basic = true, IsPrivate = true, Title = "CreateTest", MaxSets = 3, PointsToWinLastSet = 1,
                PointsToWin = 1, PointDifferenceToWin = 0, PlayersPerTeam = 0
            }, "first");
        
        Assert.AreEqual("CreateTest", result.Data.Title);
    }

    [Test]
    public async Task CreateAsyncWithEvenSetNumber_Returns400()
    {
        var result = await _gameService.CreateAsync(
            new AddGameDto()
            {
                Basic = true, IsPrivate = true, Title = "CreateTest", MaxSets = 2, PointsToWinLastSet = 1,
                PointsToWin = 1, PointDifferenceToWin = 0, PlayersPerTeam = 0
            }, "first");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task CreateAsyncWithInvalidImage_Returns400()
    {
        var result = await _gameService.CreateAsync(
            new AddGameDto()
            {
                Basic = true, IsPrivate = true, Title = "CreateTest", MaxSets = 3, PointsToWinLastSet = 1,
                PointsToWin = 1, PointDifferenceToWin = 0, PlayersPerTeam = 0, PictureUrl = "Picture"
            }, "first");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsync_ReturnsGame()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.UpdateAsync(
            new EditGameDto
            {
                Basic = true, Description = "Description", IsPrivate = true, MaxSets = 1,
                PictureUrl = "https://i.imgur.com/uOsldfd.jpeg", PlayersPerTeam = 1, PointDifferenceToWin = 1,
                PointsToWin = 1, PointsToWinLastSet = 1, Title = "Game"
            }, _games[0]);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task UpdateAsyncFinishedGame_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.UpdateAsync(
            new EditGameDto
            {
                Basic = true, Description = "Description", IsPrivate = true, MaxSets = 1,
                PictureUrl = "https://i.imgur.com/uOsldfd.jpeg", PlayersPerTeam = 1, PointDifferenceToWin = 1,
                PointsToWin = 1, PointsToWinLastSet = 1, Title = "Game"
            }, _games[2]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsyncWithEvenMaxSets_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.UpdateAsync(
            new EditGameDto
            {
                Basic = true, Description = "Description", IsPrivate = true, MaxSets = 2,
                PictureUrl = "https://i.imgur.com/uOsldfd.jpeg", PlayersPerTeam = 1, PointDifferenceToWin = 1,
                PointsToWin = 1, PointsToWinLastSet = 1, Title = "Game"
            }, _games[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsyncWithInvalidPicture_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.UpdateAsync(
            new EditGameDto
            {
                Basic = true, Description = "Description", IsPrivate = true, MaxSets = 1,
                PictureUrl = "Picture", PlayersPerTeam = 1, PointDifferenceToWin = 1,
                PointsToWin = 1, PointsToWinLastSet = 1, Title = "Game"
            }, _games[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsyncWithTournamentChangePointsToWin_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.UpdateAsync(
            new EditGameDto
            {
                Basic = true, Description = "Description", IsPrivate = true, MaxSets = 1,
                PictureUrl = "", PlayersPerTeam = 1, PointDifferenceToWin = 1,
                PointsToWin = 1, PointsToWinLastSet = 1, Title = "Game"
            }, Helpers.GetGameWithTournament());

        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsyncWithTournamentChangePointsToWinLastSet_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.UpdateAsync(
            new EditGameDto
            {
                Basic = true, Description = "Description", IsPrivate = true, MaxSets = 1,
                PictureUrl = "", PlayersPerTeam = 1, PointDifferenceToWin = 1,
                PointsToWin = null, PointsToWinLastSet = 1, Title = "Game"
            }, Helpers.GetGameWithTournament());

        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsyncWithTournamentChangePointDifferenceToWin_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.UpdateAsync(
            new EditGameDto
            {
                Basic = true, Description = "Description", IsPrivate = true, MaxSets = 1,
                PictureUrl = "", PlayersPerTeam = 1, PointDifferenceToWin = 1,
                PointsToWin = null, PointsToWinLastSet = null, Title = "Game"
            }, Helpers.GetGameWithTournament());

        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task DeleteAsync_Succeeds()
    {
        _logService.Setup(x => x.DeleteGameLogsAsync(_games[0].Id)).ReturnsAsync(ServiceResult<bool>.Success());
        _gameRepository.Setup(x => x.DeleteAsync(_games[0].Id));

        var result = await _gameService.DeleteAsync(_games[0]);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task DeleteAsyncTournamentGame_Returns400()
    {
        var result = await _gameService.DeleteAsync(Helpers.GetGameWithTournament());
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task TeamRequestJoinAsync_Succeeds()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.TeamRequestJoinAsync(_games[0], _teams[0]);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task TeamRequestJoinAsyncTournamentGame_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.TeamRequestJoinAsync(Helpers.GetGameWithTournament(), _teams[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task TeamRequestJoinAsyncAlreadyRequested_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.RequestedTeams.Add(_teams[0]);
        var result = await _gameService.TeamRequestJoinAsync(game, _teams[0]);

        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task TeamRequestJoinAsyncWrongPlayerNumber_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.TeamRequestJoinAsync(_games[1], _teams[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task TeamRequestJoinAsyncNoPlayers_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.TeamRequestJoinAsync(_games[0], Helpers.GetTeamWithNoPlayers());
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddTeamAsync_Succeeds()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.RequestedTeams.Add(_teams[0]);
        var result = await _gameService.AddTeamAsync(new AddTeamToGameDto { TeamId = _teams[0].Id }, _games[0]);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task AddTeamAsyncWithTournament_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var result = await _gameService.AddTeamAsync(new AddTeamToGameDto { TeamId = _teams[0].Id }, Helpers.GetGameWithTournamentAndRequestedTeam());
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddTeamAsyncWithoutRequest_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var result = await _gameService.AddTeamAsync(new AddTeamToGameDto { TeamId = _teams[0].Id }, _games[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddTeamAsyncWrongPlayerCount_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[1];
        var team = _teams[0];
        game.RequestedTeams.Add(team);
        var result = await _gameService.AddTeamAsync(new AddTeamToGameDto { TeamId = team.Id }, game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddTeamAsyncGameIsFull_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.RequestedTeams.Add(_teams[2]);
        game.FirstTeam = new GameTeam();
        game.SecondTeam = new GameTeam();
        var result = await _gameService.AddTeamAsync(new AddTeamToGameDto { TeamId = _teams[2].Id }, _games[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task RemoveTeamAsyncWithOneTeam_Succeeds()
    {
        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.FirstTeam = new GameTeam();

        var result = await _gameService.RemoveTeamAsync(false, game);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task RemoveTeamAsyncWithTwoTeams_Succeeds()
    {
        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.FirstTeam = new GameTeam();
        game.SecondTeam = new GameTeam();

        var result = await _gameService.RemoveTeamAsync(false, game);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task RemoveTeamAsyncWithTournament_Returns400()
    {
        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = Helpers.GetGameWithTournament();

        var result = await _gameService.RemoveTeamAsync(false, game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task RemoveTeamAsyncFromStarted_Returns400()
    {
        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.Status = GameStatus.Started;

        var result = await _gameService.RemoveTeamAsync(false, game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task RemoveTeamAsyncFirstTeamWithoutExisting_Returns400()
    {
        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];

        var result = await _gameService.RemoveTeamAsync(false, game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task RemoveTeamAsyncSecondTeamWithoutExisting_Returns400()
    {
        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];

        var result = await _gameService.RemoveTeamAsync(false, game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task StartAsync_Succeeds()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.Sets = new List<Set>();
        game.MaxSets = 5;
        game.FirstTeam = Helpers.GetGameTeam();
        game.SecondTeam = Helpers.GetGameTeam();
        game.Status = GameStatus.Ready;

        var result = await _gameService.StartAsync(game);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task StartAsyncAlreadyStarted_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.Sets = new List<Set>();
        game.MaxSets = 5;
        game.FirstTeam = Helpers.GetGameTeam();
        game.SecondTeam = Helpers.GetGameTeam();
        game.Status = GameStatus.Started;

        var result = await _gameService.StartAsync(game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task StartAsyncAlreadyFinished_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.Sets = new List<Set>();
        game.MaxSets = 5;
        game.FirstTeam = Helpers.GetGameTeam();
        game.SecondTeam = Helpers.GetGameTeam();
        game.Status = GameStatus.Finished;

        var result = await _gameService.StartAsync(game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task StartAsyncNotReady_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.Sets = new List<Set>();
        game.MaxSets = 5;
        game.FirstTeam = Helpers.GetGameTeam();
        game.SecondTeam = Helpers.GetGameTeam();
        game.Status = GameStatus.SingleTeam;

        var result = await _gameService.StartAsync(game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task StartAsyncFirstTeamWrongPlayerCount_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.Sets = new List<Set>();
        game.MaxSets = 5;
        game.PlayersPerTeam = 2;
        game.FirstTeam = Helpers.GetGameTeam(1);
        game.SecondTeam = Helpers.GetGameTeam(2);
        game.Status = GameStatus.Ready;

        var result = await _gameService.StartAsync(game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task StartAsyncSecondTeamWrongPlayerCount_Returns400()
    {
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        _gameTeamRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);

        var game = _games[0];
        game.Sets = new List<Set>();
        game.MaxSets = 5;
        game.PlayersPerTeam = 2;
        game.FirstTeam = Helpers.GetGameTeam(2);
        game.SecondTeam = Helpers.GetGameTeam(1);
        game.Status = GameStatus.Ready;

        var result = await _gameService.StartAsync(game);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task GetGameSetsAsync_ReturnsGameSets()
    {
        var game = _games[0];
        game.Sets = new List<Set> { new Set { Game = game }, new Set { Game = game }, new Set { Game = game } };
        _setRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(game.Sets);

        var result = await _gameService.GetGameSetsAsync(game.Id);
        
        Assert.AreEqual(game.Sets.Count, result.Data.Count());
    }

    [Test]
    public async Task ChangePlayerSetScoreAsyncFirstTeamLastSetIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.PointsToWinLastSet = 2;
        game.PointDifferenceToWin = 0;
        game.MaxSets = 1;
        game.Sets = new List<Set> { new Set { Id = _fakeGuid, Game = game, FirstTeamScore = 1, Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = false }}}};
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = true }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task ChangePlayerSetScoreAsyncSecondTeamLastSetIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.PointsToWinLastSet = 2;
        game.PointDifferenceToWin = 0;
        game.MaxSets = 1;
        game.Sets = new List<Set> { new Set { Id = _fakeGuid, Game = game, SecondTeamScore = 1, Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true }}}};
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = true }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task ChangePlayerSetScoreAsyncFirstTeamDecrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.Sets = new List<Set> { new Set { Id = _fakeGuid, Game = game, SecondTeamScore = 1, Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 1, Team = false }}}};
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = false }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetScoreAsyncSecondTeamDecrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.Sets = new List<Set> { new Set { Id = _fakeGuid, Game = game, SecondTeamScore = 1, Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 1, Team = true }}}};
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = false }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetScoreAsyncFirstTeamDecreaseWithScore0_Returns400()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.Sets = new List<Set> { new Set { Id = _fakeGuid, Game = game, SecondTeamScore = 1, Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = false }}}};
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = false }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task ChangePlayerSetScoreAsyncFirstTeamNotLastSetIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.PointsToWinLastSet = 2;
        game.PointDifferenceToWin = 0;
        game.MaxSets = 3;
        game.FirstTeamScore = 0;
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = false } }
            },
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 2,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = false } }
            },
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 3,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = false } }
            }
        };
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = true }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetScoreAsyncSecondTeamNotLastSetIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        var game = _games[0];
        game.PointsToWinLastSet = 2;
        game.PointDifferenceToWin = 0;
        game.MaxSets = 3;
        game.FirstTeamScore = 0;
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 2,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = false } }
            },
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 3,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = false } }
            }
        };
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = true }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task ChangePlayerSetScoreAsyncWithTournamentFirstTeamLastRound_Succeeds()
    {
        var tournament = _tournaments[0];
        tournament.FinalRound = 1;
        var tournamentMatch = tournament.Matches.First();
        _tournamentService.Setup(x => x.GetAsync(tournament.Id)).ReturnsAsync(ServiceResult<Tournament>.Success(tournament));
        _tournamentService.Setup(x => x.GetTournamentMatchesAsync(tournament.Id, true))
            .ReturnsAsync(ServiceResult<IEnumerable<TournamentMatch>>.Success(new List<TournamentMatch> { tournamentMatch}));
        _tournamentService.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync(ServiceResult<bool>.Success());
        _tournamentService.Setup(x => x.UpdateMatchAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync(ServiceResult<bool>.Success());
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        
        var game = _games[0];
        game.PointsToWinLastSet = 1;
        game.PointDifferenceToWin = 0;
        game.MaxSets = 1;
        game.FirstTeamScore = 0;
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = false } }
            },
        };
        tournamentMatch.Game = game;
        tournamentMatch.Round = 1;
        game.TournamentMatch = tournament.Matches.First();
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = true }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetScoreAsyncWithTournamentSecondTeamLastRound_Succeeds()
    {
        var tournament = _tournaments[0];
        tournament.FinalRound = 1;
        var tournamentMatch = tournament.Matches.First();
        _tournamentService.Setup(x => x.GetAsync(tournament.Id)).ReturnsAsync(ServiceResult<Tournament>.Success(tournament));
        _tournamentService.Setup(x => x.GetTournamentMatchesAsync(tournament.Id, true))
            .ReturnsAsync(ServiceResult<IEnumerable<TournamentMatch>>.Success(new List<TournamentMatch> { tournamentMatch}));
        _tournamentService.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync(ServiceResult<bool>.Success());
        _tournamentService.Setup(x => x.UpdateMatchAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync(ServiceResult<bool>.Success());
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        _gameRepository.Setup(x => x.UpdateAsync(It.IsAny<Game>())).ReturnsAsync((Game game) => game);
        
        
        var game = _games[0];
        game.PointsToWinLastSet = 1;
        game.PointDifferenceToWin = 0;
        game.MaxSets = 1;
        game.FirstTeamScore = 0;
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };
        tournamentMatch.Game = game;
        tournamentMatch.Round = 1;
        game.TournamentMatch = tournament.Matches.First();
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = true }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task ChangePlayerSetStatsAsyncKillsIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.Kills }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncErrorsIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.Errors }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncAttemptsIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.Attempts }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncSuccessfulBlocksIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.SuccessfulBlocks }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncBlocksIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.Blocks }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncTouchesIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.Touches }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncBlockingErrorsIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.BlockingErrors }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncAcesIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.Aces }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncServingErrorsIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.ServingErrors }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncTotalServesIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.TotalServes }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncSuccessfulDigsIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.SuccessfulDigs }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncBallTouchesIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.BallTouches }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task ChangePlayerSetStatsAsyncBallMissesIncrease_Succeeds()
    {
        _logService.Setup(x => x.CreateLogAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<Tournament?>(), It.IsAny<Game?>())).ReturnsAsync(ServiceResult<bool>.Success);
        _setRepository.Setup(x => x.UpdateAsync(It.IsAny<Set>())).ReturnsAsync((Set set) => set);
        
        var game = _games[0];
        game.Sets = new List<Set>
        {
            new Set
            {
                Id = _fakeGuid, Game = game, FirstTeamScore = 1, Number = 1,
                Players = new List<SetPlayer> { new SetPlayer { Id = _fakeGuid, Score = 0, Team = true } }
            },
        };

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.BallMisses }, game,
            _fakeGuid, _fakeGuid, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
}