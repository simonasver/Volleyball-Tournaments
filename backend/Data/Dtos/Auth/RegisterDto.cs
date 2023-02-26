using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Dtos.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "ApplicationUser Name is required")]
        public string UserName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email Address is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
