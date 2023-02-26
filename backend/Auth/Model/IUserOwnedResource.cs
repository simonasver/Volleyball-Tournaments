namespace Backend.Auth.Model
{
    public interface IUserOwnedResource
    {
        public string OwnerId { get; }
    }
}
