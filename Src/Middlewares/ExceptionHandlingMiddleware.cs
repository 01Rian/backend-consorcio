using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace rian_p01_back.Src.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma excecao nao tratada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode;
            object response;

            switch (exception)
            {
                case ArgumentException ex:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    response = new { message = CleanErrorMessage(ex.Message) };
                    break;
                case InvalidOperationException ex:
                    statusCode = HttpStatusCode.Conflict; // 409
                    response = new { message = ex.Message };
                    break;
                // TODO Adicionar outros casos de exceções específicas
                default:
                    statusCode = HttpStatusCode.InternalServerError; // 500
                    response = new { message = "Ocorreu um erro interno no servidor." };
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private static string CleanErrorMessage(string message)
        {
            var regex = new Regex(@"\s*\(Parameter '.*\)");
            return regex.Replace(message, "");
        }
    }
}