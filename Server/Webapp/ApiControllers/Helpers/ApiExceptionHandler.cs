using AppAPI._1._0.Responses;
using BLL.App.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapp.ApiControllers.Helpers
{
    [AllowAnonymous]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ApiExceptionHandler : ControllerBase
    {
        [Route("/api/error")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = context.Error;

            if (!context.Path.Contains("/api/")) return BadRequest();

            return exception switch
            {
                ValidationException => BadRequest(new ErrorResponseDTO(exception.Message)),
                NotFoundException => NotFound(new ErrorResponseDTO(exception.Message)),
                
                _ => BadRequest(new ErrorResponseDTO(exception.Message))
            };
        }
    }
}