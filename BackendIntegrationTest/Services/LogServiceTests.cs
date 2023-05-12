using Backend.Data;
using Backend.Data.Dtos.Game;
using Backend.Data.Dtos.Tournament;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Tournament;
using Backend.Data.Repositories;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackendIntegrationTest;

public class LogServiceTests
{
    private ApplicationDbContext _dbContext;
    private ILogRepository _logRepository;
    private ILogService _logService;
    
    private ITournamentRepository _tournamentRepository;
    private ITournamentMatchRepository _tournamentMatchRepository;
    private IGameTeamRepository _gameTeamRepository;
    private ITournamentService _tournamentService;
    
    private IGameRepository _gameRepository;
    private ISetRepository _setRepository;
    private IGameService _gameService;

    private Game _addedGame;
    private Tournament _addedTournament;

    [OneTimeSetUp]
    public void Setup()
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

        _logRepository = new LogRepository(_dbContext);
        _logService = new LogService(_logRepository);
        
        _gameTeamRepository = new GameTeamRepository(_dbContext);
        _tournamentRepository = new TournamentRepository(_dbContext);
        _tournamentMatchRepository = new TournamentMatchRepository(_dbContext);

        _tournamentService = new TournamentService(_tournamentRepository, _tournamentMatchRepository, _gameTeamRepository, _logService);

        _gameRepository = new GameRepository(_dbContext);
        _setRepository = new SetRepository(_dbContext);

        _tournamentService = new TournamentService(_tournamentRepository, _tournamentMatchRepository, _gameTeamRepository, _logService);
        _gameService = new GameService(_gameRepository, _gameTeamRepository, _setRepository, _tournamentService,
            _logService);
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _dbContext.Database.EnsureDeleted();
    }
    
    [Test, Order(1)]
    public async Task CreateLogAsync_Succeeds()
    {
        var result = await _logService.CreateLogAsync("Log", true, "first");
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(2)]
    public async Task CreateLogAsync_Game_Succeeds()
    {
        var gameResult = await _gameService.CreateAsync(addGameDto: new AddGameDto
        {
            Title = "Game",
            PictureUrl = null,
            Description = null,
            Basic = false,
            PointsToWin = 5,
            PointsToWinLastSet = 5,
            PointDifferenceToWin = 5,
            MaxSets = 5,
            PlayersPerTeam = 0,
            IsPrivate = false
        }, "first");

        var game = gameResult.Data;
        _addedGame = game;
        
        var result = await _logService.CreateLogAsync("Log", true, "first", null, game);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(3)]
    public async Task CreateLogAsync_Tournament_Succeeds()
    {
        var tournamentResult = await _tournamentService.CreateAsync(new AddTournamentDto
        {
            Title = "Tournament",
            PictureUrl = null,
            Description = null,
            Basic = false,
            SingleThirdPlace = false,
            MaxTeams = 5,
            PointsToWin = 5,
            PointsToWinLastSet = 5,
            PointDifferenceToWin = 5,
            MaxSets = 5,
            PlayersPerTeam = 0,
            IsPrivate = false
        }, "first");

        var tournament = tournamentResult.Data;
        _addedTournament = tournament;
        
        var result = await _logService.CreateLogAsync("Log", true, "first", tournament);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(4)]
    public async Task GetGameLogsAsync_ReturnsLogs()
    {
        var result = await _logService.GetGameLogsAsync(_addedGame.Id);
        
        Assert.AreEqual(1, result.Data.Count());
    }
    
    [Test, Order(5)]
    public async Task DeleteGameLogsAsync_Succeeds()
    {
        var result = await _logService.DeleteGameLogsAsync(_addedGame.Id);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(6)]
    public async Task DeleteTournamentLogsAsync_Succeeds()
    {
        var result = await _logService.DeleteTournamentLogsAsync(_addedTournament.Id);
        
        Assert.IsTrue(result.IsSuccess);
    }
}