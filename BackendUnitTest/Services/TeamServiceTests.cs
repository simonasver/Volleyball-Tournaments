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
    
    private IList<Team> _teams;

    private Guid _fakeGuid; 
    
    [SetUp]
    public void Setup()
    {
        _teamRepository = new Mock<ITeamRepository>();
        _teamService = new TeamService(_teamRepository.Object);

        _fakeGuid = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        _teams = Helpers.GetTestTeamData();
    }

    [Test]
    public async Task GetAllAsync_ReturnsAll()
    {
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        _teamRepository.Setup(x => x.GetAllAsync(searchParameters)).ReturnsAsync(_teams);

        var result = await _teamService.GetAllAsync(searchParameters);
        
        Assert.AreEqual(_teams.Count(), result.Data.Count());
    }

    [Test]
    public async Task GetUserTeamsAsync_ReturnsUserTeams()
    {
        var data = new List<Team> { _teams[0], _teams[1] };
        var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
        _teamRepository.Setup(x => x.GetAllUserAsync(searchParameters, "first")).ReturnsAsync(data);

        var result = await _teamService.GetUserTeamsAsync(searchParameters, "first");
        
        Assert.AreEqual(data.Count(), result.Data.Count());
    }

    [Test]
    public async Task GetAsync_ReturnsTeam()
    {
        _teamRepository.Setup(x => x.GetAsync(_teams[0].Id)).ReturnsAsync(_teams[0]);

        var result = await _teamService.GetAsync(_teams[0].Id);
        
        Assert.AreEqual(_teams[0].Id, result.Data.Id);
    }

    [Test]
    public async Task GetAsyncWithWrongId_Returns404()
    {
        _teamRepository.Setup(x => x.GetAsync(_fakeGuid)).ReturnsAsync((Team)null);

        var result = await _teamService.GetAsync(_teams[0].Id);
        
        Assert.AreEqual(StatusCodes.Status404NotFound, result.ErrorStatus);
    }

    [Test]
    public async Task CreateAsync_ReturnsTeam()
    {
        _teamRepository.Setup(x => x.CreateAsync(It.IsAny<Team>())).ReturnsAsync((Team team) => team);

        var result = await _teamService.CreateAsync(
            new AddTeamDto() { Title = "CreateTeam" },
            "first");
        
        Assert.AreEqual("CreateTeam", result.Data.Title);
    }

    [Test]
    public async Task CreateAsyncWithDuplicateName_Returns400()
    {
        _teamRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(_teams);

        var result = await _teamService.CreateAsync(new AddTeamDto() { Title = "Team1" }, "first");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task CreateAsyncWithInvalidImage_Returns400()
    {
        var result = await _teamService.CreateAsync(new AddTeamDto() { Title = "Team1", PictureUrl = "Picture"}, "first");
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsync_Succeeds()
    {
        _teamRepository.Setup(x => x.UpdateAsync(It.IsAny<Team>())).ReturnsAsync((Team team) => team);

        var result = await _teamService.UpdateAsync(
            new EditTeamDto() { Description = _teams[0].Description, PictureUrl = _teams[0].PictureUrl, Title = _teams[0].Title },
            _teams[0]);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task UpdateAsyncWithDuplicateName_Returns400()
    {
        _teamRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(_teams);

        var result = await _teamService.UpdateAsync(new EditTeamDto() { Title = "Team1" }, _teams[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task UpdateAsyncWithInvalidImage_Returns400()
    {
        var result = await _teamService.UpdateAsync(new EditTeamDto() { Title = "Team1", PictureUrl = "Picture"}, _teams[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task DeleteAsync_Succeeds()
    {
        _teamRepository.Setup(x => x.DeleteAsync(_teams[0].Id));

        var result = await _teamService.DeleteAsync(_teams[0].Id);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task AddPlayerAsync_Succeeds()
    {
        _teamRepository.Setup(x => x.UpdateAsync(_teams[0])).ReturnsAsync(_teams[0]);

        var result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto() { Name = "Player" }, _teams[0]);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task AddPlayerAsyncWithFullTeam_Returns400()
    {
        var teamWith11Players = _teams[0];
        for (int i = 0; i < 11; i++)
        {
            teamWith11Players.Players.Add(new TeamPlayer { Id = new Guid(), Name = "Player" + i, Team = teamWith11Players} );
        }

        var result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto {Name = "Player13"}, _teams[0]);

        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task AddPlayerAsyncWithDuplicateName_Returns400()
    {
        var result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto { Name = "Player1" }, _teams[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }

    [Test]
    public async Task RemovePlayerAsync_Succeeds()
    {
        _teamRepository.Setup(x => x.UpdateAsync(_teams[0])).ReturnsAsync(_teams[0]);

        var result = await _teamService.RemovePlayerAsync(_teams[0].Players.First().Id, _teams[0]);
        
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task RemovePlayerAsyncWithWrongId_Returns400()
    {
        var result = await _teamService.RemovePlayerAsync(_fakeGuid, _teams[0]);
        
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
    }
}