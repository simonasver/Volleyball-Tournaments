using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Dtos.Auth
{
    public class LoginDtoRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class LoginDtoResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string UserEmail { get; set; }
        public IList<string> UserRoles { get; set; }
    
        public LoginDtoResponse(string accessToken, string refreshToken, string userId, string profilePictureUrl, string userName, string fullName, string userEmail,
            IList<string> userRoles)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            UserId = userId;
            ProfilePictureUrl = profilePictureUrl;
            UserName = userName;
            FullName = fullName;
            UserEmail = userEmail;
            UserRoles = userRoles;
        }
    }
}
