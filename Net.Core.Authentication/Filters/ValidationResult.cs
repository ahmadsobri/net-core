using Microsoft.AspNetCore.Mvc.ModelBinding;
using Net.Core.Authentication.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Net.Core.Authentication.Filters
{
    public class ValidationResult
    {
        public string Status { get; }
        public string Message { get; }

        public ValidationResult(ModelStateDictionary modelState)
        {
            IEnumerable<string> errorMessages = modelState.Values.SelectMany(c => c.Errors.Select(x => x.ErrorMessage));
            string msg = errorMessages.FirstOrDefault();

            //var keys = modelState.Where(m => m.Value.Errors.Count() > 0).Select(c=>c.Key.Split(".")?.Last()).ToList();
            //string msg = modelState.Where(modelError => modelError.Value.Errors.Count > 0)
            //    .Select(modelError => modelError.Value.Errors.FirstOrDefault().ErrorMessage)
            //    .FirstOrDefault();
            //string msg = String.Format("error validation of '{0}'", String.Join("', '", keys));


            Status = ResponseCode.Fail;
            Message = msg;
        }
    }
}
