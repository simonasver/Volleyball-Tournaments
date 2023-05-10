using Backend.Data.Entities.Auth;

namespace Backend.Interfaces.Services;

public interface IUserManagedResource : IUserOwnedResource
{
    public ICollection<ApplicationUser> Managers { get; }
}