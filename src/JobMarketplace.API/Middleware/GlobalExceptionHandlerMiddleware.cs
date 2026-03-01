using JobMarketplace.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace JobMarketplace.API.Middleware
{
    /// <summary>
    /// Catches all unhandled exceptions and returns consistent JSON error responses.
    /// ValidationException → 400, NotFoundException → 404, everything else → 500.
    /// Registered first in the pipeline so it wraps all controllers and handlers.
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
                // Correlation ID is available because CorrelationIdMiddleware runs before this
                var correlationId = context.Items["CorrelationId"]?.ToString() ?? "unknown";

                _logger.LogError(ex,
                    "Unhandled exception on {Method} {Path} | CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    correlationId);

                await HandleExceptionAsync(context, ex, correlationId);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            context.Response.ContentType = "application/json";

            // Pattern match exception type → HTTP status code + response body
            var (statusCode, response) = exception switch
            {
                ValidationException validationEx => (
                    (int)HttpStatusCode.BadRequest,
                    (object)new { error = "Validation Failed", details = validationEx.Errors, correlationId }
                ),
                NotFoundException notFoundEx => (
                    (int)HttpStatusCode.NotFound,
                    (object)new { error = notFoundEx.Message, correlationId }
                ),
                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    (object)new { error = "An unexpected error occurred.", correlationId }  // CorrelationId helps support trace the issue
                )
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}