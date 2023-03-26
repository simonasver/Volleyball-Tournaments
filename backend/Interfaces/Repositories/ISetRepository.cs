using Backend.Data.Entities.Game;

namespace Backend.Interfaces.Repositories;

public interface ISetRepository
{
    public Task<IEnumerable<Set>> GetAllAsync();
    public Task<Set?> GetAsync(Guid setId);
    public Task<Set> CreateAsync(Set set);
    public Task<Set> UpdateAsync(Set set);
    public Task DeleteAsync(Guid setId);
}