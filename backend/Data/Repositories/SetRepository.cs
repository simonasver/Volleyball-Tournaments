using Backend.Data.Entities.Game;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class SetRepository : ISetRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SetRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<IEnumerable<Set>> GetAllAsync()
    {
        return await _dbContext.Sets.Include(x => x.Players).Include(x => x.Game).OrderBy(x => x.Number).ToListAsync();
    }

    public async Task<Set> UpdateAsync(Set set)
    {
        _dbContext.Sets.Update(set);
        await _dbContext.SaveChangesAsync();
        return set;
    }
}