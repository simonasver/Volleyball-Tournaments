using System.Collections;
using Backend.Data.Dtos.Tournament;
using Backend.Data.Entities.Auth;
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

public class TournamentServiceTests
{
    private ITournamentService _tournamentService;
    private Mock<ITournamentRepository> _tournamentRepository;
    private Mock<ITournamentMatchRepository> _tournamentMatchRepository;
    private Mock<IGameTeamRepository> _gameTeamRepository;
    private Mock<ILogService> _logService;
    
    private IList<Tournament> _tournaments;
    private IList<Team> _teams;
    
    private Guid _fakeGuid;
    
    [SetUp]
    public void Setup()
    {
        _tournamentRepository = new Mock<ITournamentRepository>();
        _tournamentMatchRepository = new Mock<ITournamentMatchRepository>();
        _gameTeamRepository = new Mock<IGameTeamRepository>();
        _logService = new Mock<ILogService>();
        _tournamentService =
            new TournamentService(_tournamentRepository.Object, _tournamentMatchRepository.Object, _gameTeamRepository.Object, _logService.Object);

        _fakeGuid = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        
        _tournaments = Helpers.GetTestTournamentData();
        _teams = Helpers.GetTestTeamData();
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllTournaments()
    {
        var tournaments = _tournaments;
        _tournamentRepository.Setup(x => x.GetAllAsync(It.IsAny<bool>(), It.IsAny<SearchParameters>()))
            .ReturnsAsync(tournaments);
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        
        var result = await _tournamentService.GetAllAsync(true, searchParameters);
        
        Assert.AreEqual(tournaments.Count, result.Data.Count());
    }

    [Test]
    public async Task GetUserTournamentsAsync_ReturnsAllUserTournaments()
    {
        var tournaments = _tournaments;
        _tournamentRepository.Setup(x => x.GetAllUserAsync(It.IsAny<SearchParameters>(), It.IsAny<string>()))
            .ReturnsAsync(tournaments);
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        
        var result = await _tournamentService.GetUserTournamentsAsync(searchParameters, "user");
        
        Assert.AreEqual(tournaments.Count, result.Data.Count());
    }

    [Test]
    public async Task GetAsync_ReturnsTournament()
    {
        var tournament = _tournaments[0];
        _tournamentRepository.Setup(x => x.GetAsync(tournament.Id)).ReturnsAsync(tournament);

        var result = await _tournamentService.GetAsync(tournament.Id);
        
        Assert.AreEqual(tournament.Id, result.Data.Id);
    }

    [Test]
    public async Task GetAsync_WithWrongId_Returns404()
    {
        var tournament = _tournaments[0];
        _tournamentRepository.Setup(x => x.GetAsync(tournament.Id)).ReturnsAsync((Tournament?)null);

        var result = await _tournamentService.GetAsync(tournament.Id);
        
        Assert.AreEqual(StatusCodes.Status404NotFound, result.ErrorStatus);
    }

    [Test]
    public async Task GetTournamentMatchesAsync_ReturnsTournamentMatches()
    {
        var tournamentMatches = (IList<TournamentMatch>)Helpers.GetTestTournamentData()[0].Matches;
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(tournamentMatches);

        var result = await _tournamentService.GetTournamentMatchesAsync(new Guid(), true);
        
        Assert.AreEqual(tournamentMatches.Count, result.Data.Count());
    }

    [Test]
    public async Task CreateAsync_Succeeds()
    {
        _tournamentRepository.Setup(x => x.CreateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.CreateAsync(new AddTournamentDto { MaxSets = 1 }, "user");
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task CreateAsync_WithWrongMaxSets_Returns400()
    {
        _tournamentRepository.Setup(x => x.CreateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.CreateAsync(new AddTournamentDto { MaxSets = 2 }, "user");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task CreateAsync_WithInvalidPicture_Returns400()
    {
        _tournamentRepository.Setup(x => x.CreateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.CreateAsync(new AddTournamentDto { MaxSets = 1, PictureUrl = "picture" }, "user");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsync_WithoutParameters_Succeeds()
    {
        var tournament = _tournaments[0];
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.UpdateAsync(tournament);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task UpdateAsync_WithParameters_Succeeds()
    {
        var tournament = _tournaments[0];
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.UpdateAsync(new EditTournamentDto
        {
            Title = "1",
            PictureUrl = "https://s.imgur.com/images/logo-1200-630.jpg?2",
            Description = "1",
            SingleThirdPlace = false,
            Basic = false,
            MaxTeams = 1,
            PointsToWin = 1,
            PointsToWinLastSet = 1,
            PointDifferenceToWin = 1,
            MaxSets = 1,
            PlayersPerTeam = 1,
            IsPrivate = false
        }, tournament);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task UpdateAsync_WithParametersWithFinishedStatus_Returns400()
    {
        var tournament = _tournaments[0];
        tournament.Status = TournamentStatus.Finished;
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.UpdateAsync(new EditTournamentDto { }, tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsync_WithParametersWithInvalidMaxSets_Returns400()
    {
        var tournament = _tournaments[0];
        tournament.Status = TournamentStatus.Finished;
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.UpdateAsync(new EditTournamentDto { MaxSets = 2 }, tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsync_WithParametersWithInvalidPicture_Returns400()
    {
        var tournament = _tournaments[0];
        tournament.Status = TournamentStatus.Finished;
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.UpdateAsync(new EditTournamentDto { PictureUrl = "picture"}, tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateMatchAsync_Succeeds()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch match) => match);

        var result = await _tournamentService.UpdateMatchAsync(new TournamentMatch());
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task DeleteAsync_Succeeds()
    {
        _logService.Setup(x => x.DeleteTournamentLogsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(ServiceResult<bool>.Success());
        _tournamentRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()));

        var result = await _tournamentService.DeleteAsync(_tournaments[0]);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task TeamRequestJoinAsync_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.RequestedTeams = new List<Team>();
        var team = _teams[0];
        var result = await _tournamentService.TeamRequestJoinAsync(tournament, team);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task TeamRequestJoinAsync_AlreadyRequested_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        var team = _teams[0];
        tournament.RequestedTeams = new List<Team> { team };
        var result = await _tournamentService.TeamRequestJoinAsync(tournament, team);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task TeamRequestJoinAsync_InvalidPlayerCount_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.AcceptedTeams = new List<GameTeam>();
        tournament.RequestedTeams = new List<Team>();
        tournament.PlayersPerTeam = 2;
        var team = _teams[0];
        var result = await _tournamentService.TeamRequestJoinAsync(tournament, team);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task TeamRequestJoinAsync_NoPlayers_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.AcceptedTeams = new List<GameTeam>();
        tournament.RequestedTeams = new List<Team>();
        tournament.PlayersPerTeam = 0;
        var team = _teams[0];
        team.Players = new List<TeamPlayer>();
        var result = await _tournamentService.TeamRequestJoinAsync(tournament, team);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddTeamAsync_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        tournament.MaxTeams = 100;
        var team = _teams[0];
        tournament.AcceptedTeams = new List<GameTeam>();
        tournament.RequestedTeams = new List<Team> { team };
        var result = await _tournamentService.AddTeamAsync(new AddTeamToTournamentDto { TeamId = team.Id }, tournament);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task AddTeamAsync_TournamentFull_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        tournament.MaxTeams = 1;
        var team = _teams[0];
        tournament.RequestedTeams = new List<Team> { team };
        tournament.AcceptedTeams = new List<GameTeam> { Helpers.GetGameTeam() };
        var result = await _tournamentService.AddTeamAsync(new AddTeamToTournamentDto { TeamId = team.Id }, tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddTeamAsync_NotRequested_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        tournament.MaxTeams = 100;
        var team = _teams[0];
        tournament.AcceptedTeams = new List<GameTeam>();
        tournament.RequestedTeams = new List<Team>();
        var result = await _tournamentService.AddTeamAsync(new AddTeamToTournamentDto { TeamId = team.Id }, tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddTeamAsync_InvalidPlayerCount_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        tournament.MaxTeams = 100;
        tournament.PlayersPerTeam = 10;
        var team = _teams[0];
        tournament.RequestedTeams = new List<Team> { team };
        tournament.AcceptedTeams = new List<GameTeam>();
        var result = await _tournamentService.AddTeamAsync(new AddTeamToTournamentDto { TeamId = team.Id }, tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task RemoveTeamAsync_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        var team = Helpers.GetGameTeam(0);
        tournament.AcceptedTeams = new List<GameTeam> { team };
        var result = await _tournamentService.RemoveTeamAsync(tournament, team.Id);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task RemoveTeamAsync_TournamentStarted_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        tournament.Status = TournamentStatus.Started;
        var team = Helpers.GetGameTeam(0);
        tournament.AcceptedTeams = new List<GameTeam> { team };
        var result = await _tournamentService.RemoveTeamAsync(tournament, team.Id);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task StartAsync_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        var team = Helpers.GetGameTeam(0);
        tournament.AcceptedTeams = new List<GameTeam> { team, team };
        var result = await _tournamentService.StartAsync(tournament);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task StartAsync_SingleThirdPlace_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        tournament.SingleThirdPlace = true;
        var team = Helpers.GetGameTeam(0);
        tournament.AcceptedTeams = new List<GameTeam> { team, team };
        var result = await _tournamentService.StartAsync(tournament);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task StartAsync_AlreadyStarted_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        tournament.Status = TournamentStatus.Started;
        tournament.AcceptedTeams = new List<GameTeam>();
        var result = await _tournamentService.StartAsync(tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task StartAsync_AlreadyFinished_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        tournament.Status = TournamentStatus.Finished;
        tournament.AcceptedTeams = new List<GameTeam>();
        var result = await _tournamentService.StartAsync(tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task StartAsync_InvalidTeamCount_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);
        
        var tournament = _tournaments[0];
        var team = Helpers.GetGameTeam(0);
        tournament.AcceptedTeams = new List<GameTeam> { team };
        var result = await _tournamentService.StartAsync(tournament);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task MoveBracketAsync_Succeeds()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch tournamentMatch) => tournamentMatch);
        
        var tournament = _tournaments[0];
        var matches = new List<TournamentMatch>(tournament.Matches);
        matches[2].FirstParent = matches[0];
        matches[2].FirstParentId = matches[0].Id;
        matches[2].SecondParent = matches[1];
        matches[2].SecondParentId = matches[1].Id;
        matches[0].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        matches[1].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        matches[2].Game = new Game { };
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((IList<TournamentMatch>)matches);

        var result = await _tournamentService.MoveBracketAsync(tournament, matches[1].Id);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task MoveBracketAsync_FromFinalRound_Returns400()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch tournamentMatch) => tournamentMatch);
        
        var tournament = _tournaments[0];
        var matches = new List<TournamentMatch>(tournament.Matches);
        matches[2].FirstParent = matches[0];
        matches[2].FirstParentId = matches[0].Id;
        matches[2].SecondParent = matches[1];
        matches[2].SecondParentId = matches[1].Id;
        matches[0].Round = 1;
        matches[1].Round = 1;
        matches[2].Round = 2;
        matches[0].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        matches[1].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        matches[2].Game = new Game { };
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((IList<TournamentMatch>)matches);

        var result = await _tournamentService.MoveBracketAsync(tournament, matches[2].Id);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task MoveBracketAsync_HasHaveOpponent_Returns400()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch tournamentMatch) => tournamentMatch);
        
        var tournament = _tournaments[0];
        var matches = new List<TournamentMatch>(tournament.Matches);
        matches[2].FirstParent = matches[0];
        matches[2].FirstParentId = matches[0].Id;
        matches[2].SecondParent = matches[1];
        matches[2].SecondParentId = matches[1].Id;
        matches[0].Round = 2;
        matches[1].Round = 2;
        matches[2].Round = 3;
        matches[0].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        matches[1].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        matches[2].Game = new Game { };
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((IList<TournamentMatch>)matches);

        var result = await _tournamentService.MoveBracketAsync(tournament, matches[0].Id);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task MoveBracketAsync_WillHaveOpponent_Returns400()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch tournamentMatch) => tournamentMatch);
        
        var tournament = _tournaments[0];
        var matches = new List<TournamentMatch>(tournament.Matches);
        matches[2].FirstParent = matches[0];
        matches[2].FirstParentId = matches[0].Id;
        matches[2].SecondParent = matches[1];
        matches[2].SecondParentId = matches[1].Id;
        matches[1].SecondParent = matches[3];
        matches[1].SecondParentId = matches[3].Id;
        matches[0].Round = 2;
        matches[1].Round = 2;
        matches[2].Round = 3;
        matches[0].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        matches[1].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        matches[2].Game = new Game { };
        matches[3].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((IList<TournamentMatch>)matches);

        var result = await _tournamentService.MoveBracketAsync(tournament, matches[1].Id);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task MoveBracketAsync_AlreadyMovedDownFirstTeamFirstParent_Returns400()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch tournamentMatch) => tournamentMatch);
        
        var tournament = _tournaments[0];
        var matches = new List<TournamentMatch>(tournament.Matches);
        matches[2].FirstParent = matches[0];
        matches[2].FirstParentId = matches[0].Id;
        matches[2].SecondParent = matches[1];
        matches[2].SecondParentId = matches[1].Id;
        matches[0].FirstParent = matches[3];
        matches[0].FirstParentId = matches[3].Id;
        matches[0].Round = 2;
        matches[1].Round = 2;
        matches[2].Round = 3;
        matches[0].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        matches[1].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        matches[2].Game = new Game { };
        matches[3].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((IList<TournamentMatch>)matches);

        var result = await _tournamentService.MoveBracketAsync(tournament, matches[3].Id);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task MoveBracketAsync_AlreadyMovedDownFirstTeamSecondParent_Returns400()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch tournamentMatch) => tournamentMatch);
        
        var tournament = _tournaments[0];
        var matches = new List<TournamentMatch>(tournament.Matches);
        matches[2].FirstParent = matches[0];
        matches[2].FirstParentId = matches[0].Id;
        matches[2].SecondParent = matches[1];
        matches[2].SecondParentId = matches[1].Id;
        matches[0].SecondParent = matches[3];
        matches[0].SecondParentId = matches[3].Id;
        matches[0].Round = 2;
        matches[1].Round = 2;
        matches[2].Round = 3;
        matches[0].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        matches[1].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        matches[2].Game = new Game { };
        matches[3].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((IList<TournamentMatch>)matches);

        var result = await _tournamentService.MoveBracketAsync(tournament, matches[3].Id);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task MoveBracketAsync_AlreadyMovedDownSecondTeamFirstParent_Returns400()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch tournamentMatch) => tournamentMatch);
        
        var tournament = _tournaments[0];
        var matches = new List<TournamentMatch>(tournament.Matches);
        matches[2].FirstParent = matches[0];
        matches[2].FirstParentId = matches[0].Id;
        matches[2].SecondParent = matches[1];
        matches[2].SecondParentId = matches[1].Id;
        matches[0].FirstParent = matches[3];
        matches[0].FirstParentId = matches[3].Id;
        matches[0].Round = 2;
        matches[1].Round = 2;
        matches[2].Round = 3;
        matches[0].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        matches[1].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        matches[2].Game = new Game { };
        matches[3].Game = new Game { SecondTeam = Helpers.GetGameTeam(1) };
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((IList<TournamentMatch>)matches);

        var result = await _tournamentService.MoveBracketAsync(tournament, matches[3].Id);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task MoveBracketAsync_AlreadyMovedDownSecondTeamSecondParent_Returns400()
    {
        _tournamentMatchRepository.Setup(x => x.UpdateAsync(It.IsAny<TournamentMatch>()))
            .ReturnsAsync((TournamentMatch tournamentMatch) => tournamentMatch);
        
        var tournament = _tournaments[0];
        var matches = new List<TournamentMatch>(tournament.Matches);
        matches[2].FirstParent = matches[0];
        matches[2].FirstParentId = matches[0].Id;
        matches[2].SecondParent = matches[1];
        matches[2].SecondParentId = matches[1].Id;
        matches[0].SecondParent = matches[3];
        matches[0].SecondParentId = matches[3].Id;
        matches[0].Round = 2;
        matches[1].Round = 2;
        matches[2].Round = 3;
        matches[0].Game = new Game { FirstTeam = Helpers.GetGameTeam(1), SecondTeam = Helpers.GetGameTeam(1) };
        matches[1].Game = new Game { FirstTeam = Helpers.GetGameTeam(1) };
        matches[2].Game = new Game { };
        matches[3].Game = new Game { SecondTeam = Helpers.GetGameTeam(1) };
        _tournamentMatchRepository.Setup(x => x.GetAllTournamentAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((IList<TournamentMatch>)matches);

        var result = await _tournamentService.MoveBracketAsync(tournament, matches[3].Id);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task GenerateAsync_Succeeds()
    {
        _tournamentRepository.Setup(x => x.CreateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.GenerateAsync(30, "user");
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task GenerateAsync_InvalidTeamCount_Returns400()
    {
        _tournamentRepository.Setup(x => x.CreateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament tournament) => tournament);

        var result = await _tournamentService.GenerateAsync(300, "user");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task AddManager_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>())).ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.Managers = new List<ApplicationUser>();
        
        var result = await _tournamentService.AddManagerAsync(tournament, new ApplicationUser { Id = "second" });
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task AddManager_IsOwner_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>())).ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.OwnerId = "first";
        tournament.Managers = new List<ApplicationUser>();
        
        var result = await _tournamentService.AddManagerAsync(tournament, new ApplicationUser { Id = "first" });
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddManager_AlreadyManager_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>())).ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.Managers = new List<ApplicationUser>{ new() { Id = "first" }};
        
        var result = await _tournamentService.AddManagerAsync(tournament, new ApplicationUser { Id = "first" });
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task RemoveManager_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>())).ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.Managers = new List<ApplicationUser>{ new() { Id = "second" }};
        
        var result = await _tournamentService.RemoveManagerAsync(tournament, new ApplicationUser { Id = "second" });
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    [Test]
    public async Task RemoveManager_IsOwner_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>())).ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.OwnerId = "first";
        tournament.Managers = new List<ApplicationUser>{ new() { Id = "first" }};
        
        var result = await _tournamentService.RemoveManagerAsync(tournament, new ApplicationUser { Id = "first" });
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
    
    [Test]
    public async Task RemoveManager_IsNotManager_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>())).ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.Managers = new List<ApplicationUser>{ new () { Id = "first" }};
        
        var result = await _tournamentService.RemoveManagerAsync(tournament, new ApplicationUser { Id = "second" });
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task ReorderTeams_Succeeds()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>())).ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.AcceptedTeams = new List<GameTeam> { new() { Id = _fakeGuid }, new() { Id = new Guid() } };

        var result =
            await _tournamentService.ReorderTeamsAsync(tournament, new Dictionary<Guid, int> { { _fakeGuid, 2 } });
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task ReorderTeams_TournamentStarted_Returns400()
    {
        _tournamentRepository.Setup(x => x.UpdateAsync(It.IsAny<Tournament>())).ReturnsAsync((Tournament tournament) => tournament);

        var tournament = _tournaments[0];
        tournament.AcceptedTeams = new List<GameTeam> { new() { Id = _fakeGuid }, new() { Id = new Guid() } };
        tournament.Status = TournamentStatus.Started;

        var result =
            await _tournamentService.ReorderTeamsAsync(tournament, new Dictionary<Guid, int> { { _fakeGuid, 2 } });
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
}