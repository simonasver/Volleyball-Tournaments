using Backend.Data;
using Backend.Data.Dtos.Team;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Utils;
using Backend.Data.Repositories;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace BackendIntegrationTest
{
    public class TeamServiceTests
    {
        private ApplicationDbContext _dbContext;
        private ITeamRepository _teamRepository;
        private ITeamService _teamService;

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

            _teamRepository = new TeamRepository(_dbContext);
            _teamService = new TeamService(_teamRepository);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Test, Order(1)]
        public async Task CreateAsync_ReturnsTeam()
        {
            var result = await _teamService.CreateAsync(
                new AddTeamDto() { Title = "CreateTeam" },
                "first");
        
            Assert.AreEqual("CreateTeam", result.Data.Title);
        }

        [Test, Order(2)]
        public async Task CreateAsync_WithDuplicateName_Returns400()
        {
            var result = await _teamService.CreateAsync(new AddTeamDto() { Title = "CreateTeam" }, "first");
        
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
        }
        
        [Test, Order(3)]
        public async Task UpdateAsync_Succeeds()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
            var team = (await _teamService.GetAllAsync(searchParameters)).Data.First();
            
            var result = await _teamService.UpdateAsync(
                new EditTeamDto() { Description = "desc" },
                team);
        
            Assert.IsTrue(result.IsSuccess);
        }

        [Test, Order(4)]
        public async Task GetAllAsync_ReturnsAll()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };

            var result = await _teamService.GetAllAsync(searchParameters);
        
            Assert.AreEqual(1, result.Data.Count());
        }

        [Test, Order(5)]
        public async Task GetUserTeamsAsync_ReturnsUserTeams()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };

            var result = await _teamService.GetUserTeamsAsync(searchParameters, "first");
        
            Assert.AreEqual(1, result.Data.Count());
        }

        [Test, Order(6)]
        public async Task GetAsync_ReturnsTeam()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
            var team = (await _teamService.GetAllAsync(searchParameters)).Data.First();

            var result = await _teamService.GetAsync(team.Id);
        
            Assert.AreEqual(team.Id, result.Data.Id);
        }

        [Test, Order(7)]
        public async Task GetAsync_WithWrongId_Returns404()
        {
            var result = await _teamService.GetAsync(new Guid());
        
            Assert.AreEqual(StatusCodes.Status404NotFound, result.ErrorStatus);
        }
        
        [Test, Order(8)]
        public async Task AddPlayerAsync_Succeeds()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
            var team = (await _teamService.GetAllAsync(searchParameters)).Data.First();

            var result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto() { Name = "Player" }, team);
        
            Assert.IsTrue(result.IsSuccess);
        }

        [Test, Order(9)]
        public async Task AddPlayerAsync_WithFullTeam_Returns400()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
            var team = (await _teamService.GetAllAsync(searchParameters)).Data.First();
            ServiceResult<bool> result = new ServiceResult<bool>();
            
            for (int i = 0; i < 12; i++)
            {
                team.Players.Add(new TeamPlayer { Id = new Guid(), Name = "Player" + i, Team = team} );
                result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto {Name = "Player" + i}, team);
            }
            
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
        }

        [Test, Order(10)]
        public async Task AddPlayerAsync_WithDuplicateName_Returns400()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
            var team = (await _teamService.GetAllAsync(searchParameters)).Data.First();
            
            var result = await _teamService.AddPlayerAsync(new AddTeamPlayerDto { Name = "Player1" }, team);
        
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
        }

        [Test, Order(11)]
        public async Task RemovePlayerAsync_Succeeds()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
            var team = (await _teamService.GetAllAsync(searchParameters)).Data.First();

            var result = await _teamService.RemovePlayerAsync(team.Players.First().Id, team);
        
            Assert.IsTrue(result.IsSuccess);
        }

        [Test, Order(12)]
        public async Task RemovePlayerAsync_WithWrongId_Returns400()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
            var team = (await _teamService.GetAllAsync(searchParameters)).Data.First();
            
            var result = await _teamService.RemovePlayerAsync(new Guid(), team);
        
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.ErrorStatus);
        }
        
        [Test, Order(13)]
        public async Task DeleteAsync_Succeeds()
        {
            var searchParameters = new SearchParameters() { PageNumber = 1, PageSize = 10 };
            var team = (await _teamService.GetAllAsync(searchParameters)).Data.First();
            
            var result = await _teamService.DeleteAsync(team.Id);
        
            Assert.IsTrue(result.IsSuccess);
        }
    }
}