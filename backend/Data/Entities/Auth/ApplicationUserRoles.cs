namespace Backend.Data.Entities.Auth;

public class ApplicationUserRoles
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);

    public static readonly IReadOnlyCollection<string> All = new[] { Admin, User };
}

