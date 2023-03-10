using System;

namespace Net.Core.Authentication.Entities
{
    public class Role
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
