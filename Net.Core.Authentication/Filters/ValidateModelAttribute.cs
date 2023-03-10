using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Net.Core.Authentication.Filters;
using System.Collections.Generic;
using System.Linq;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new ValidationFailedResult(context.ModelState);

            //IEnumerable<string> errorMessages = context.ModelState.Values.SelectMany(modelState => modelState.Errors.Select(x => x.ErrorMessage));
            //context.Result = new BadRequestObjectResult(errorMessages);
        }
    }
}