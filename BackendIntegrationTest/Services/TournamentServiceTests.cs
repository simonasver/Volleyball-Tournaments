using Backend.Data;
using Backend.Data.Dtos.Tournament;
using Backend.Data.Entities.Auth;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;
using Backend.Data.Repositories;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackendIntegrationTest;

public class TournamentServiceTests
{
    private ApplicationDbContext _dbContext;
    private ITournamentRepository _tournamentRepository;
    private ITournamentMatchRepository _tournamentMatchRepository;
    private IGameTeamRepository _gameTeamRepository;
    private ILogRepository _logRepository;
    private ILogService _logService;
    private ITournamentService _tournamentService;

    private Tournament _addedTournament;
    private Team _joinedTeam1;
    private Team _joinedTeam2;
    private Team _joinedTeam3;

    private Guid _addedNotOwnerUserGuid;
    private ApplicationUser _addedNotOwnerUser;
    
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
        
        _addedNotOwnerUserGuid = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        _addedNotOwnerUser = new ApplicationUser
        {
            Id = _addedNotOwnerUserGuid.ToString(),
            UserName = "notadmin",
            NormalizedUserName = null,
            Email = "notadmin@admin.com",
            NormalizedEmail = null,
            EmailConfirmed = false,
            PasswordHash = null,
            SecurityStamp = null,
            ConcurrencyStamp = null,
            PhoneNumber = null,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            RefreshToken = null,
            RefreshTokenExpiration = default,
            ProfilePictureUrl = null,
            FullName = "not",
            RegisterDate = default,
            LastLoginDate = default,
            Banned = false,
            OwnedTeams = null,
            OwnedGames = null,
            OwnedTournaments = null,
            ManagedTeams = null,
            ManagedGames = null,
            ManagedTournaments = null
        };

        _gameTeamRepository = new GameTeamRepository(_dbContext);
        _tournamentRepository = new TournamentRepository(_dbContext);
        _tournamentMatchRepository = new TournamentMatchRepository(_dbContext);
        _logRepository = new LogRepository(_dbContext);

        _logService = new LogService(_logRepository);
        _tournamentService = new TournamentService(_tournamentRepository, _tournamentMatchRepository, _gameTeamRepository, _logService);
        _tournamentService = new TournamentService(_tournamentRepository, _tournamentMatchRepository, _gameTeamRepository, _logService);

        _joinedTeam1 = (await _dbContext.Teams.AddAsync(new Team { Title = "Team", Players = new List<TeamPlayer> { new() { Name = "Player" }}, OwnerId = "first" })).Entity;
        _joinedTeam2 = (await _dbContext.Teams.AddAsync(new Team { Title = "Team1", Players = new List<TeamPlayer> { new() { Name = "Player" }}, OwnerId = "first" })).Entity;
        _joinedTeam3 = (await _dbContext.Teams.AddAsync(new Team { Title = "Team2", Players = new List<TeamPlayer> { new() { Name = "Player" }}, OwnerId = "first" })).Entity;
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _dbContext.Database.EnsureDeleted();
    }
    
    [Test, Order(1)]
    public async Task CreateAsync_Succeeds()
    {
        var result = await _tournamentService.CreateAsync(new AddTournamentDto
        {
            Title = "Tournament",
            PictureUrl = null,
            Description = null,
            Basic = false,
            SingleThirdPlace = false,
            MaxTeams = 128,
            PointsToWin = 2,
            PointsToWinLastSet = 2,
            PointDifferenceToWin = 2,
            MaxSets = 5,
            PlayersPerTeam = 0,
            IsPrivate = false
        }, "first");
        _addedTournament = result.Data;
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(2)]
    public async Task UpdateAsync_WithoutParameters_Succeeds()
    {
        var result = await _tournamentService.UpdateAsync(_addedTournament);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(3)]
    public async Task UpdateAsync_WithParameters_Succeeds()
    {
        var result = await _tournamentService.UpdateAsync(new EditTournamentDto
        {
            Title = "1",
            PictureUrl = "https://s.imgur.com/images/logo-1200-630.jpg?2",
            Description = "1",
            SingleThirdPlace = false,
            Basic = false,
            MaxTeams = 128,
            PointsToWin = 2,
            PointsToWinLastSet = 2,
            PointDifferenceToWin = 2,
            MaxSets = 5,
            PlayersPerTeam = 0,
            IsPrivate = false
        }, _addedTournament);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(4)]
    public async Task TeamRequestJoinAsync_Succeeds()
    {
        var tournament = (await _tournamentService.GetAsync(_addedTournament.Id)).Data;
        
        var result = await _tournamentService.TeamRequestJoinAsync(tournament, _joinedTeam1);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(5)]
    public async Task AddTeamAsync_Succeeds()
    {
        var tournament = (await _tournamentService.GetAsync(_addedTournament.Id)).Data;
        
        var result = await _tournamentService.AddTeamAsync(new AddTeamToTournamentDto { TeamId = _joinedTeam1.Id }, tournament);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(6)]
    public async Task RemoveTeamAsync_Succeeds()
    {
        var tournament = (await _tournamentService.GetAsync(_addedTournament.Id)).Data;
        
        var result = await _tournamentService.RemoveTeamAsync(tournament, tournament.AcceptedTeams.First().Id);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(7)]
    public async Task ReorderTeams_Succeeds()
    {
        await _tournamentService.TeamRequestJoinAsync(_addedTournament, _joinedTeam1);
        await _tournamentService.TeamRequestJoinAsync(_addedTournament, _joinedTeam2);
        await _tournamentService.TeamRequestJoinAsync(_addedTournament, _joinedTeam3);
        await _tournamentService.AddTeamAsync(new AddTeamToTournamentDto { TeamId = _joinedTeam1.Id }, _addedTournament);
        await _tournamentService.AddTeamAsync(new AddTeamToTournamentDto { TeamId = _joinedTeam2.Id }, _addedTournament);
        await _tournamentService.AddTeamAsync(new AddTeamToTournamentDto { TeamId = _joinedTeam3.Id },
            _addedTournament);
        
        var updatedTournament = (await _tournamentService.GetAsync(_addedTournament.Id)).Data;

        var result =
            await _tournamentService.ReorderTeamsAsync(_addedTournament, new Dictionary<Guid, int> { { updatedTournament.AcceptedTeams[0].Id, 2 } });
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(8)]
    public async Task StartAsync_Succeeds()
    {
        var result = await _tournamentService.StartAsync(_addedTournament);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test, Order(9)]
    public async Task AddManager_Succeeds()
    {
        var result = await _tournamentService.AddManagerAsync(_addedTournament, _addedNotOwnerUser);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(10)]
    public async Task RemoveManager_Succeeds()
    {
        var result = await _tournamentService.RemoveManagerAsync(_addedTournament, _addedNotOwnerUser);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(11)]
    public async Task GetAllAsync_ReturnsAllTournaments()
    {
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        
        var result = await _tournamentService.GetAllAsync(true, searchParameters);
        
        Assert.AreEqual(1, result.Data.Count());
    }

    [Test, Order(12)]
    public async Task GetUserTournamentsAsync_ReturnsAllUserTournaments()
    {
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        
        var result = await _tournamentService.GetUserTournamentsAsync(searchParameters, "first");
        
        Assert.AreEqual(1, result.Data.Count());
    }

    [Test, Order(13)]
    public async Task GetAsync_ReturnsTournament()
    {
        var result = await _tournamentService.GetAsync(_addedTournament.Id);
        
        Assert.AreEqual(_addedTournament.Id, result.Data.Id);
    }

    [Test, Order(14)]
    public async Task GetAsync_WithWrongId_Returns404()
    {
        var result = await _tournamentService.GetAsync(new Guid());
        
        Assert.AreEqual(StatusCodes.Status404NotFound, result.ErrorStatus);
    }

    [Test, Order(15)]
    public async Task GetTournamentMatchesAsync_ReturnsTournamentMatches()
    {
        var result = await _tournamentService.GetTournamentMatchesAsync(_addedTournament.Id, true);
        
        Assert.AreEqual(3, result.Data.Count());
    }
    
    [Test, Order(16)]
    public async Task MoveBracketAsync_Succeeds()
    {
        var matches = await _tournamentService.GetTournamentMatchesAsync(_addedTournament.Id, true);
        var result = await _tournamentService.MoveBracketAsync(_addedTournament, matches.Data.ToList()[2].Id);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test, Order(17)]
    public async Task DeleteAsync_Succeeds()
    {
        var result = await _tournamentService.DeleteAsync(_addedTournament);
        
        Assert.IsTrue(result.IsSuccess);
    }
}