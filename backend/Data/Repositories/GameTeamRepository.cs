using Backend.Data.Entities.Game;
using Backend.Data.Entities.Tournament;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameTeamRepository : IGameTeamRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public GameTeamRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<IEnumerable<GameTeam>> GetAllAsync()
    {
        return await _dbContext.GameTeams.ToListAsync();
    }

    public async Task<GameTeam?> GetAsync(Guid gameTeamId)
    {
        return await _dbContext.GameTeams.FirstOrDefaultAsync(x => x.Id == gameTeamId);
    }

    public async Task<GameTeam> CreateAsync(GameTeam gameTeam)
    {
        _dbContext.GameTeams.Add(gameTeam);
        await _dbContext.SaveChangesAsync();
        return gameTeam;
    }

    public async Task<GameTeam> UpdateAsync(GameTeam gameTeam)
    {
        _dbContext.GameTeams.Update(gameTeam);
        await _dbContext.SaveChangesAsync();
        return gameTeam;
    }

    public async Task DeleteAsync(Guid gameTeamId)
    {
        var gameTeamToDelete = await _dbContext.GameTeams.FirstOrDefaultAsync(x => x.Id == gameTeamId);
        if (gameTeamToDelete != null)
        {
            _dbContext.GameTeams.Remove(gameTeamToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}