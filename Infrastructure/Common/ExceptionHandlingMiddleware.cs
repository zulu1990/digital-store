using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Common
{
    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);

                _logger.LogError($"exception happened: {ex.Message}");

            }
        }

        private static async Task HandleException(HttpContext context, Exception ex)
        {
            var statusCode = 500;
            var response = new
            {
                status = statusCode,
                details = ex.Message,
                errors = GetErrors(ex)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }


        private static IEnumerable<string> GetErrors(Exception exception)
        {
            IEnumerable<string> errors = null;

            if (exception is ValidationException validationException)
            {
                errors = validationException.Errors.Select(x => x.ErrorMessage);
            }

            return errors;
        }

    }
}
