using Net.Core.Authentication.Entities.Contexts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Net.Core.Authentication.Auth
{
    public class AccessTokenBuilder
    {
        private readonly Settings _appSettings;
        private readonly string payload;
        public AccessTokenBuilder(Settings appSettings, string payload)
        {
            _appSettings = appSettings;
            this.payload = payload;
        }

        public string GenerateAccessToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
            string pattern = String.Format("{0}:{1}:{2}", payload, key, DateTime.Now);
            string token = string.Empty;
            using (var hmac = new System.Security.Cryptography.HMACSHA512(key))
            {
                token = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(pattern)));
            }
            return token;
        }
        public string GenerateRefreshToken()
        {
            var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
            string pattern = String.Format("{0}:{1}:{2}", DateTime.Now, key, payload);
            string token = string.Empty;
            using (var hmac = new System.Security.Cryptography.HMACSHA512(key))
            {
                token = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(pattern)));
            }
            return token;
        }
    }
}
