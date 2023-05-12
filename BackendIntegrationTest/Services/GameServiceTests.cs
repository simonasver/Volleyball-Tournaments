using Backend.Data;
using Backend.Data.Dtos.Game;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;
using Backend.Data.Repositories;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackendIntegrationTest;

public class GameServiceTests
{
    private ApplicationDbContext _dbContext;
    private IGameRepository _gameRepository;
    private IGameTeamRepository _gameTeamRepository;
    private ISetRepository _setRepository;
    private ITournamentRepository _tournamentRepository;
    private ITournamentMatchRepository _tournamentMatchRepository;
    private ILogRepository _logRepository;
    private ILogService _logService;
    private IGameService _gameService;
    private ITournamentService _tournamentService;

    private Game _addedGame;
    private Team _joinedTeam1;
    private Team _joinedTeam2;

    [OneTimeSetUp]
    public async Task Setup()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var connectionString = configuration.GetConnectionString("TestConnection");

        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder =
            new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql(
                connectionString, ServerVersion.AutoDetect(connectionString));

        using (var dbContext = new ApplicationDbContext(configuration, optionsBuilder.Options))
        {
            dbContext.Database.Migrate();
        }

        _dbContext = new ApplicationDbContext(configuration, optionsBuilder.Options);

        _dbContext.Database.ExecuteSql(
            $"INSERT INTO aspnetusers (Id, UserName, FullName, Email, RefreshTokenExpiration, RegisterDate, LastLoginDate, Banned, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount) VALUES ({"first"}, {"admin"}, {"Admin admin"}, {"admin@admin.com"}, {DateTime.Now}, {DateTime.Now}, {DateTime.Now}, {false}, {false}, {false}, {false}, {false}, {0})");

        _gameRepository = new GameRepository(_dbContext);
        _gameTeamRepository = new GameTeamRepository(_dbContext);
        _setRepository = new SetRepository(_dbContext);
        _tournamentRepository = new TournamentRepository(_dbContext);
        _tournamentMatchRepository = new TournamentMatchRepository(_dbContext);
        _logRepository = new LogRepository(_dbContext);

        _logService = new LogService(_logRepository);
        _tournamentService = new TournamentService(_tournamentRepository, _tournamentMatchRepository, _gameTeamRepository, _logService);
        _gameService = new GameService(_gameRepository, _gameTeamRepository, _setRepository, _tournamentService,
            _logService);

        _joinedTeam1 = (await _dbContext.Teams.AddAsync(new Team { Title = "Team", Players = new List<TeamPlayer> { new() { Name = "Player" }}, OwnerId = "first" })).Entity;
        _joinedTeam2 = (await _dbContext.Teams.AddAsync(new Team { Title = "Team1", Players = new List<TeamPlayer> { new() { Name = "Player" }}, OwnerId = "first" })).Entity;
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _dbContext.Database.EnsureDeleted();
    }
    
    [Test, Order(1)]
    public async Task CreateAsync_ReturnsGame()
    {
        var result = await _gameService.CreateAsync(
            new AddGameDto()
            {
                Basic = false, IsPrivate = true, Title = "CreateTest", MaxSets = 3, PointsToWinLastSet = 1,
                PointsToWin = 1, PointDifferenceToWin = 0, PlayersPerTeam = 0
            }, "first");
        _addedGame = result.Data;
        
        Assert.AreEqual("CreateTest", result.Data.Title);
    }
    
    [Test, Order(2)]
    public async Task GetAllAsync_ReturnsAll()
    {
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };

        var result = await _gameService.GetAllAsync(true, searchParameters);
        
        Assert.AreEqual(1, result.Data.Count());
    }

    [Test, Order(3)]
    public async Task GetUserGamesAsync_ReturnsUserGames()
    {
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };

        var result = await _gameService.GetUserGamesAsync(searchParameters, "first");
        
        Assert.AreEqual(1, result.Data.Count());
    }

    [Test, Order(4)]
    public async Task GetAsync_ReturnsGame()
    {
        var result = await _gameService.GetAsync(_addedGame.Id);
        
        Assert.AreEqual(_addedGame.Id, result.Data.Id);
    }
    
    [Test, Order(5)]
    public async Task UpdateAsync_ReturnsGame()
    {
        var result = await _gameService.UpdateAsync(
            new EditGameDto
            {
                Basic = false, Description = "Description", IsPrivate = true, MaxSets = 1,
                PictureUrl = "https://i.imgur.com/uOsldfd.jpeg", PlayersPerTeam = 1, PointDifferenceToWin = 2,
                PointsToWin = 2, PointsToWinLastSet = 1, Title = "Game"
            }, _addedGame);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(6)]
    public async Task TeamRequestJoinAsync_Succeeds()
    {
        var game = await _gameRepository.GetAsync(_addedGame.Id);
        
        var result = await _gameService.TeamRequestJoinAsync(game, _joinedTeam1);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test, Order(7)]
    public async Task TeamRequestJoinAsync_AlreadyRequested_Returns400()
    {
        var game = await _gameRepository.GetAsync(_addedGame.Id);

        var result = await _gameService.TeamRequestJoinAsync(game, _joinedTeam1);
    
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test, Order(8)]
    public async Task AddTeamAsync_Succeeds()
    {
        var game = await _gameRepository.GetAsync(_addedGame.Id);
        
        var result = await _gameService.AddTeamAsync(new AddTeamToGameDto { TeamId = _joinedTeam1.Id }, game);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(9)]
    public async Task RemoveTeamAsync_WithOneTeam_Succeeds()
    {
        var game = await _gameRepository.GetAsync(_addedGame.Id);

        var result = await _gameService.RemoveTeamAsync(false, game);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(10)]
    public async Task StartAsync_Succeeds()
    {
        var game = await _gameRepository.GetAsync(_addedGame.Id);
        await _gameService.TeamRequestJoinAsync(game, _joinedTeam1);
        await _gameService.TeamRequestJoinAsync(game, _joinedTeam2);
        await _gameService.AddTeamAsync(new AddTeamToGameDto { TeamId = _joinedTeam1.Id }, game);
        await _gameService.AddTeamAsync(new AddTeamToGameDto { TeamId = _joinedTeam2.Id }, game);

        var result = await _gameService.StartAsync(game);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(11)]
    public async Task GetGameSetsAsync_ReturnsGameSets()
    {
        var game = await _gameRepository.GetAsync(_addedGame.Id);

        var result = await _gameService.GetGameSetsAsync(game.Id);
        
        Assert.AreEqual(game.Sets.Count, result.Data.Count());
    }
    
    [Test, Order(12)]
    public async Task ChangePlayerSetScoreAsync_FirstTeamLastSetIncrease_Succeeds()
    {
        var game = await _gameRepository.GetAsync(_addedGame.Id);
        
        var result = await _gameService.ChangePlayerSetScoreAsync(new ChangeSetPlayerScoreDto { Change = true }, game,
            game.Sets.First().Id, game.Sets.First().Players.First().Id, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(13)]
    public async Task ChangePlayerSetStatsAsync_KillsIncrease_Succeeds()
    {
        var game = await _gameRepository.GetAsync(_addedGame.Id);

        var result = await _gameService.ChangePlayerSetStatsAsync(new ChangeSetPlayerStatsDto { Change = true, Type = SetPlayerStats.Kills }, game,
            game.Sets.First().Id, game.Sets.First().Players.First().Id, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(14)]
    public async Task DeleteAsync_Succeeds()
    {
        var result = await _gameService.DeleteAsync(_addedGame);
        
        Assert.IsTrue(result.IsSuccess);
    }
}