using Net.Core.Authentication.Enum;
using Net.Core.Authentication.Exceptions.Base;
using System;

namespace Net.Core.Authentication.Exceptions
{
    public class SignatureException : Exception, IBaseException
    {
        private string Status { get; set; }
        private string Message { get; set; }

        public SignatureException(string message)
        {
            Status = ResponseCode.Fail;
            Message = message;
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
