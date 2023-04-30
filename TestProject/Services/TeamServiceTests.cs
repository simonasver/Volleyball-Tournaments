using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;
using Backend.Data.Repositories;
using Backend.Helpers.Extensions;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace TestProject.Services;

public class TeamServiceTests
{
    private ITeamService _teamService;
    private Mock<ITeamRepository> _teamRepository;
    private Team _team1, _team2, _team3;
    private TeamPlayer _player1, _player2, _player3;

    private Guid _fakeGuid; 
    
    [SetUp]
    public void Setup()
    {
        _teamRepository = new Mock<ITeamRepository>();
        _teamService = new TeamService(_teamRepository.Object);

        _fakeGuid = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        
        _team1 = new Team()
        {
            Id = new Guid(),
            Title = "Team1",
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            OwnerId = "first",
            Players = new List<TeamPlayer>()
        };
        _team2 = new Team(){
            Id = new Guid(),
            Title = "Team2",
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            OwnerId = "first",
            Players = new List<TeamPlayer>()
        };
        _team3 = new Team(){
            Id = new Guid(),
            Title = "Team3",
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            OwnerId = "second",
            Players = new List<TeamPlayer>()
        };
        
        _player1 = new TeamPlayer()
        {
            Id = new Guid(),
            Name = "Player1",
            Team = _team1
        };
        _team1.Players.Add(_player1);
        _player2 = new TeamPlayer()
        {
            Id = new Guid(),
            Name = "Player2",
            Team = _team2
        };
        _team2.Players.Add(_player2);
        _player3 = new TeamPlayer()
        {
            Id = new Guid(),
            Name = "Player3",
            Team = _team3
        };
        _team3.Players.Add(_player3);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAll()
    {
        var data = new List<Team> { _team1, _team2, _team3 };
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        _teamRepository.Setup(x => x.GetAllAsync(searchParameters)).ReturnsAsync(data);

        var result = await _teamService.GetAllAsync(searchParameters);
        
        Assert.AreEqual(result.Data.Count(), data.Count());
    }

    [Test]
    public async Task GetUserTeamsAsync_ReturnsUserTeams()
    {
        var data = new List<Team> { _team1, _team2 };
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        _teamRepository.Setup(x => x.GetAllUserAsync(searchParameters, "first")).ReturnsAsync(data);

        var result = await _teamService.GetUserTeamsAsync(searchParameters, "first");
        
        Assert.AreEqual(result.Data.Count(), data.Count());
    }

    [Test]
    public async Task GetAsync_ReturnsTeam()
    {
        _teamRepository.Setup(x => x.GetAsync(_team1.Id)).ReturnsAsync(_team1);

        var result = await _teamService.GetAsync(_team1.Id);
        
        Assert.AreEqual(result.Data.Id, _team1.Id);
    }

    [Test]
    public async Task GetAsyncWithWrongId_Returns404()
    {
        _teamRepository.Setup(x => x.GetAsync(_team1.Id)).ReturnsAsync((Team)null);

        var result = await _teamService.GetAsync(_team1.Id);
        
        Assert.AreEqual(result.ErrorStatus, StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task CreateAsync_ReturnsTeam()
    {
        _teamRepository.Setup(x => x.CreateAsync(It.IsAny<Team>())).ReturnsAsync(_team1);

        var result = await _teamService.CreateAsync(
            new AddTeamDto() { Description = _team1.Description, PictureUrl = _team1.PictureUrl, Title = _team1.Title },
            "first");
        
        Assert.AreEqual(result.Data.Title, _team1.Title);
    }

    [Test]
    public async Task CreateAsyncWithDuplicateName_Returns400()
    {
        _teamRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Team> {_team1, _team2, _team3});

        var result = await _teamService.CreateAsync(new AddTeamDto() { Title = "Team1" }, "first");
        
        Assert.AreEqual(result.ErrorStatus, StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task CreateAsyncWithInvalidImage_Returns400()
    {
        var result = await _teamService.CreateAsync(new AddTeamDto() { Title = "Team1", PictureUrl = "Picture"}, "first");
        
        Assert.AreEqual(result.ErrorStatus, StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task UpdateAsync_Succeeds()
    {
        _teamRepository.Setup(x => x.UpdateAsync(_team1)).ReturnsAsync(_team1);

        var result = await _teamService.UpdateAsync(
            new EditTeamDto() { Description = _team1.Description, PictureUrl = _team1.PictureUrl, Title = _team1.Title },
            _team1);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task UpdateAsyncWithDuplicateName_Returns400()
    {
        _teamRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Team> {_team1, _team2, _team3});

        var result = await _teamService.UpdateAsync(new EditTeamDto() { Title = "Team1" }, _team1);
        
        Assert.AreEqual(result.ErrorStatus, StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task UpdateAsyncWithInvalidImage_Returns400()
    {
        var result = await _teamService.UpdateAsync(new EditTeamDto() { Title = "Team1", PictureUrl = "Picture"}, _team1);
        
        Assert.AreEqual(result.ErrorStatus, StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task DeleteAsync_Succeeds()
    {
        _teamRepository.Setup(x => x.DeleteAsync(_team1.Id));

        var result = await _teamService.DeleteAsync(_team1.Id);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task AddPlayerAsync_Succeeds()
    {
        _teamRepository.Setup(x => x.UpdateAsync(_team1)).ReturnsAsync(_team1);

        var result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto() { Name = "Player" }, _team1);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task AddPlayerAsyncWithFullTeam_Returns400()
    {
        var teamWith11Players = _team1;
        for (int i = 0; i < 11; i++)
        {
            teamWith11Players.Players.Add(new TeamPlayer { Id = new Guid(), Name = "Player" + i, Team = teamWith11Players} );
        }

        var result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto {Name = "Player13"}, _team1);

        Assert.AreEqual(result.ErrorStatus, StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task AddPlayerAsyncWithDuplicateName_Returns400()
    {
        var result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto { Name = "Player1" }, _team1);
        
        Assert.AreEqual(result.ErrorStatus, StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task RemovePlayerAsync_Succeeds()
    {
        _teamRepository.Setup(x => x.UpdateAsync(_team1)).ReturnsAsync(_team1);

        var result = await _teamService.RemovePlayerAsync(_team1.Players.First().Id, _team1);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task RemovePlayerAsyncWithWrongId_Returns400()
    {
        var result = await _teamService.RemovePlayerAsync(_fakeGuid, _team1);
        
        Assert.AreEqual(result.ErrorStatus, StatusCodes.Status400BadRequest);
    }
}