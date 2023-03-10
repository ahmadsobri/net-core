namespace Net.Core.Authentication.Enum
{
    public class ResponseCode
    {
        public static readonly string Unauthorized = "99";
        public static readonly string Ok = "00";
        public static readonly string Fail = "50";
    }

    public class UserStatus
    {
        public static readonly int Active = 1;
        public static readonly int Inactive = 0;
    }

    public class TokenType
    {
        public static readonly string Bearer = "Bearer";
        public static readonly string Basic = "Basic";
    }
}
