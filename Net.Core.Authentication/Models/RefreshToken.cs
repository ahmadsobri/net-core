using System.ComponentModel.DataAnnotations;

namespace Net.Core.Authentication.Models
{
    public class RefreshToken
    {
        [Required(ErrorMessage = "grant type is required")]
        [RegularExpression(@"^refresh_token$", ErrorMessage = "grant type not valid")]
        public string grant_type { get; set; }
        [Required(ErrorMessage = "refresh token is required")]
        public string refresh_token { get; set; }
    }
}
