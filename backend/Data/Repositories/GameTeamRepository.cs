using Backend.Data.Entities.Game;
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