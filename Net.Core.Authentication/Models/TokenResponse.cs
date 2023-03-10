﻿using System;

namespace Net.Core.Authentication.Models
{
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public double expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
