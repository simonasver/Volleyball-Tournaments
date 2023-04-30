using Backend.Auth.Model;

namespace Backend.Data.Dtos.User;

public class AddRemoveRoleDto
{
    public string Role { get; set; }
    public bool Add { get; set; }
}