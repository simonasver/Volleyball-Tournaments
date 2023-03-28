using System.Collections;
using Backend.Data.Entities.Game;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameRepository : IGameRepository
{
    private readonly ApplicationDbContext _dbContext;

    public GameRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _dbContext.Games.ToListAsync();
    }

    public async Task<Game?> GetAsync(Guid gameId)
    {
        return await _dbContext.Games.
            Include(x => x.RequestedTeams)
                .ThenInclude(x => x.Players)
            .Include(x => x.BlockedTeams)
            .Include(x => x.FirstTeam)
                .ThenInclude(x => x.Players)
            .Include(x => x.SecondTeam)
                .ThenInclude(x => x.Players)
            .Include(x => x.Sets)
            .Include(x => x.Winner)
            .FirstOrDefaultAsync(x => x.Id == gameId);
    }

    public async Task<Game> CreateAsync(Game game)
    {
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();
        return game;
    }

    public async Task<Game> UpdateAsync(Game game)
    {
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
        return game;
    }

    public async Task DeleteAsync(Guid gameId)
    {
        var gameToDelete = await _dbContext.Games.FirstOrDefaultAsync(x => x.Id == gameId);
        if (gameToDelete != null)
        {
            _dbContext.Games.Remove(gameToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}