using Backend.Data.Entities;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Log;
using Backend.Data.Entities.Tournament;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace TestProject.Services;

public class LogServiceTests
{
    private Mock<ILogRepository> _logRepository;
    private ILogService _logService;

    private IList<Log> _logs;
    private IList<Game> _games;
    private IList<Tournament> _tournaments;
    
    [SetUp]
    public void Setup()
    {
        _logRepository = new Mock<ILogRepository>();
        _logService = new LogService(_logRepository.Object);

        _logs = Helpers.GetTestLogData();
        _games = Helpers.GetTestGameData();
        _tournaments = Helpers.GetTestTournamentData();
    }

    [Test]
    public async Task CreateLogAsync_Succeeds()
    {
        _logRepository.Setup(x => x.CreateAsync(It.IsAny<Log>())).ReturnsAsync((Log log) => log);

        var result = await _logService.CreateLogAsync("Log", true, "user");
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task CreateLogAsyncWithoutMessage_Returns400()
    {
        _logRepository.Setup(x => x.CreateAsync(It.IsAny<Log>())).ReturnsAsync((Log log) => log);

        var result = await _logService.CreateLogAsync(null, true, "user");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task GetGameLogsAsync_ReturnsLogs()
    {
        var game = _games[0];
        var logs = _logs;
        foreach (var log in logs)
        {
            log.Game = game;
        }
        _logRepository.Setup(x => x.GetAllGame(It.IsAny<Guid>())).ReturnsAsync(logs);

        var result = await _logService.GetGameLogsAsync(game.Id);
        
        Assert.AreEqual(logs.Count, result.Data.Count());
    }
    
    [Test]
    public async Task DeleteGameLogsAsync_Succeeds()
    {
        var game = _games[0];
        var logs = _logs;
        foreach (var log in logs)
        {
            log.Game = game;
        }

        _logRepository.Setup(x => x.DeleteAllGameAsync(It.IsAny<Guid>()));

        var result = await _logService.DeleteGameLogsAsync(game.Id);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task DeleteTournamentLogsAsync_Succeeds()
    {
        var tournament = _tournaments[0];
        var logs = _logs;
        foreach (var log in logs)
        {
            log.Tournament = tournament;
        }

        _logRepository.Setup(x => x.DeleteAllTournamentAsync(It.IsAny<Guid>()));

        var result = await _logService.DeleteTournamentLogsAsync(tournament.Id);
        
        Assert.IsTrue(result.IsSuccess);
    }
}