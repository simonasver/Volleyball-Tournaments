using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Dtos.Auth
{
    public class LoginDtoRequest
    {
        [Required(ErrorMessage = "ApplicationUser Name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email Address is required")]
        public string Password { get; set; }
    }

    public class LoginDtoResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public IList<string> UserRoles { get; set; }
    
        public LoginDtoResponse(string accessToken, string refreshToken, string userName, string userEmail,
            IList<string> userRoles)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            UserName = userName;
            UserEmail = userEmail;
            UserRoles = userRoles;
        }
    }
}
