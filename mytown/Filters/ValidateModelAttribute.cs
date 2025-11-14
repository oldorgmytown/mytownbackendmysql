using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace mytown.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .Select(ms => new
                    {
                        Field = ms.Key,
                        Error = ms.Value.Errors.First().ErrorMessage
                    });

                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = errors
                });
            }
        }
    }
}

