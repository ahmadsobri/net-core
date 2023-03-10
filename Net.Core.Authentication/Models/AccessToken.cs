using System.ComponentModel.DataAnnotations;

namespace Net.Core.Authentication.Models
{
    public class AccessToken
    {        
        [Required(ErrorMessage = "grant type is required")]
        [RegularExpression(@"^password$", ErrorMessage = "grant type not valid")]
        public string grant_type { get; set; }
        [Required(ErrorMessage = "username is required")]
        public string username { get; set; }

        [Required(ErrorMessage = "password is required")]
        public string password { get; set; }
    }
}
