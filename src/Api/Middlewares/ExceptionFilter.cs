using Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Middlewares
{
    public class ExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public ExceptionFilter()
        {
        }

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is System.Exception exception)
            {
                var objectResult = new Result<bool>(false);
                objectResult.AddError("unexpected_exception", exception.Message.ToString());

                context.Result = new JsonResult(objectResult)
                {
                    StatusCode = 500
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
