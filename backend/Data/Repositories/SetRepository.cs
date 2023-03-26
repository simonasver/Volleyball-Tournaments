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
        return await _dbContext.Sets.ToListAsync();
    }

    public async Task<Set?> GetAsync(Guid setId)
    {
        return await _dbContext.Sets.FirstOrDefaultAsync(x => x.Id == setId);
    }

    public async Task<Set> CreateAsync(Set set)
    {
        _dbContext.Sets.Add(set);
        await _dbContext.SaveChangesAsync();
        return set;
    }

    public async Task<Set> UpdateAsync(Set set)
    {
        _dbContext.Sets.Update(set);
        await _dbContext.SaveChangesAsync();
        return set;
    }

    public async Task DeleteAsync(Guid setId)
    {
        var setToDelete = await _dbContext.Sets.FirstOrDefaultAsync(x => x.Id == setId);
        if (setToDelete != null)
        {
            _dbContext.Sets.Remove(setToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}