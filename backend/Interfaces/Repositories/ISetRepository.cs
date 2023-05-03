using Backend.Data.Entities.Game;

namespace Backend.Interfaces.Repositories;

public interface ISetRepository
{
    public Task<IEnumerable<Set>> GetAllAsync();
    public Task<Set> UpdateAsync(Set set);
}