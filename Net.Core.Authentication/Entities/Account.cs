using System;

namespace Net.Core.Authentication.Entities
{
    public class Account
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public string account_number { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
