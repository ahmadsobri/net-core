using System;

namespace Net.Core.Authentication.Entities
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string key_secret { get; set; }
        public string key_signature { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int? status { get; set; }
        public DateTime? access_token_generated { get; set; }
        public DateTime? access_token_expired { get; set; }
        public DateTime? refresh_token_generated { get; set; }
        public DateTime? refresh_token_expired { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

    }
}
