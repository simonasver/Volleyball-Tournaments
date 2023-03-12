namespace Backend.Data.Dtos.User;

public class GetUserDto
{
    public string ProfilePictureUrl { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public IList<string> UserRoles { get; set; }

    public GetUserDto(string profilePictureUrl, string userName, string fullName, string email, DateTime registerDate,
        DateTime lastLoginDate, IList<string> userRoles)
    {
        ProfilePictureUrl = profilePictureUrl;
        UserName = userName;
        FullName = fullName;
        Email = email;
        RegisterDate = registerDate;
        LastLoginDate = lastLoginDate;
        UserRoles = userRoles;
    }
}