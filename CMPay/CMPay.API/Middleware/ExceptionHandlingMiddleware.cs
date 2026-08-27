using CMPay.Application.Exceptions;

namespace CMPay.API.Middleware
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
            catch (NotFoundException ex)
            {
                await WriteResponseAsync(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (BusinessException ex)
            {
                await WriteResponseAsync(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado ao processar a requisição.");
                await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado ao processar a requisição.");
            }
        }

        private static Task WriteResponseAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(new { message });
        }
    }
}
