namespace JobMarketplace.API.Middleware
{
    /// <summary>
    /// Assigns a unique Correlation ID to every request.
    /// If the caller sends X-Correlation-Id, we reuse it (service-to-service tracing).
    /// Otherwise we generate one. The ID is added to the response header and
    /// pushed into Serilog's LogContext so every log entry includes it automatically.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Use caller's correlation ID if provided, otherwise generate one
            var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                ?? Guid.NewGuid().ToString("N");

            // Store in HttpContext.Items so other middleware/services can access it
            context.Items["CorrelationId"] = correlationId;

            // Add to response header so the caller can trace it
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            // Push into Serilog's LogContext — every log entry in this request gets the ID
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}