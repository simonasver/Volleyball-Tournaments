using Backend.Data.Entities.Game;
using Backend.Interfaces.Repositories;

namespace Backend.Data.Repositories;

public class SetRepository : ISetRepository
{
    public async Task<IEnumerable<Set>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Set?> GetAsync(Guid setId)
    {
        throw new NotImplementedException();
    }

    public async Task<Set> CreateAsync(Set set)
    {
        throw new NotImplementedException();
    }

    public async Task<Set> UpdateAsync(Set set)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Set set)
    {
        throw new NotImplementedException();
    }
}