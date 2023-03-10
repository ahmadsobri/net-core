using System;

namespace Net.Core.Authentication.Entities
{
    public class Log
    {
        public string id { get; set; }
        public string request_url { get; set; }
        public DateTime? request_time { get; set; }
        public DateTime? response_time { get; set; }
        public long? elapsed_time { get; set; }
        public string request_header { get; set; }
        public string request_body { get; set; }
        public string response_header { get; set; }
        public string response_body { get; set; }
        public string response_code { get; set; }
    }
}
