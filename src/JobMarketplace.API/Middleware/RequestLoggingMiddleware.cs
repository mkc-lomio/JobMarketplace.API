using System.Diagnostics;

namespace JobMarketplace.API.Middleware
{
    /// <summary>
    /// Logs every HTTP request: method, path, status code, and duration in ms.
    /// Does NOT log request/response bodies (security risk — passwords, tokens).
    /// Works alongside the MediatR LoggingBehavior:
    ///   - This middleware catches the HTTP level (all requests including static files, auth failures)
    ///   - LoggingBehavior catches the MediatR level (handler execution details)
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                await _next(context);
                sw.Stop();

                var statusCode = context.Response.StatusCode;
                var level = statusCode >= 500 ? LogLevel.Error
                          : statusCode >= 400 ? LogLevel.Warning
                          : LogLevel.Information;

                _logger.Log(level,
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    sw.ElapsedMilliseconds);
            }
            catch (Exception)
            {
                sw.Stop();
                _logger.LogError(
                    "HTTP {Method} {Path} threw exception after {ElapsedMs}ms",
                    context.Request.Method,
                    context.Request.Path,
                    sw.ElapsedMilliseconds);
                throw; // Re-throw — GlobalExceptionHandlerMiddleware handles it
            }
        }
    }
}