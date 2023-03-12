using Microsoft.AspNetCore.Identity;

namespace Backend.Auth.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string FullName { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
