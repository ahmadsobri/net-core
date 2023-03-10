using System.ComponentModel.DataAnnotations;

namespace Net.Core.Authentication.Models
{
    public class Register
    {
        [Required(ErrorMessage = "username is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; }
    }
}
