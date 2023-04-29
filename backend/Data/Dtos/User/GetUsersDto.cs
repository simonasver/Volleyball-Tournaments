namespace Backend.Data.Dtos.User;

public class GetUsersUserDto
{
    public string Id { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public IList<string> Roles { get; set; }
    public bool Banned { get; set; }

    public GetUsersUserDto(string id, string profilePictureUrl, string userName, string fullName, string email, DateTime registerDate,
        DateTime lastLoginDate, IList<string> userRoles, bool banned)
    {
        Id = id;
        ProfilePictureUrl = profilePictureUrl;
        UserName = userName;
        FullName = fullName;
        Email = email;
        RegisterDate = registerDate;
        LastLoginDate = lastLoginDate;
        Roles = userRoles;
        Banned = banned;
    }
}