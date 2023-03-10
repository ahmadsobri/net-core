using Net.Core.Authentication.Enum;
using Net.Core.Authentication.Exceptions.Base;
using System;

namespace Net.Core.Authentication.Exceptions
{
    public class AppException : Exception, IBaseException
    {
        private string Status { get; set; }
        private string Message { get; set; }
        public AppException(string status, string message)
        {
            Status = status;
            Message = message;
        }

        public AppException(string message){
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
