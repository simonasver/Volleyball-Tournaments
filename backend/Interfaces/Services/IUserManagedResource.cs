namespace Backend.Interfaces.Services;

public interface IUserManagerResource : IUserOwnedResource
{
    public ICollection<string> ManagerIds { get; }
}