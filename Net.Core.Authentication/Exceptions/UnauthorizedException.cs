using Net.Core.Authentication.Enum;
using Net.Core.Authentication.Exceptions.Base;
using System;

namespace Net.Core.Authentication.Exceptions
{
    public class UnauthorizedException : Exception, IBaseException
    {
        private string Status { get; set; }
        private string Message { get; set; }

        public UnauthorizedException()
        {
            Status = ResponseCode.Unauthorized;
            Message = "Unauthorized";
        }

        public string GetStatus()
        {
            return Status;
        }

        public string GetMessage()
        {
            return Message;
        }
    }
}
