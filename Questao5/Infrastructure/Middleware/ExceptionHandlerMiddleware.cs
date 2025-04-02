using System.Net;
using Newtonsoft.Json;
using Questao5.Domain.Exception;
using Questao5.Infrastructure.Swagger;

namespace Questao5.Infrastructure.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            // Mapeando exceções para códigos HTTP apropriados
            var statusCode = exception switch
            {
                Domain.Exception.ApplicationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            response.StatusCode = statusCode;
            var errorType = ApplicationErrorGenerator.INTERNAL_ERROR;
            if (exception is Domain.Exception.ApplicationException)
                errorType = ((Domain.Exception.ApplicationException)exception).ErrorType;

            var errorResponse = new ErrorResponse(errorType, exception.Message);

            return response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}
