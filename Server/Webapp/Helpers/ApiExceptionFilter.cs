using AppAPI._1._0.Responses;
using BLL.Base.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Webapp.ApiControllers.Helpers
{
    public class ApiExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                context.Result = new JsonResult(new ErrorResponseDTO(validationException.Message))
                {
                    StatusCode = 400,
                };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is NotFoundException notFoundException)
            {
                context.Result = new JsonResult(new ErrorResponseDTO(notFoundException.Message))
                {
                    StatusCode = 404,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}