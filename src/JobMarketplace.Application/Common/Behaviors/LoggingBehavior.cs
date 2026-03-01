using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace JobMarketplace.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior — logs every request with execution time.
    /// Warns if a handler takes longer than 500ms (potential performance issue).
    /// Pipeline: Request → ValidationBehavior → [LoggingBehavior] → Handler
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private const int SlowThresholdMs = 500;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Handling {RequestName} | {@Request}", requestName, request);

            var sw = Stopwatch.StartNew();
            var response = await next();
            sw.Stop();

            // Warn if handler took longer than threshold — helps catch slow queries early
            if (sw.ElapsedMilliseconds > SlowThresholdMs)
            {
                _logger.LogWarning(
                    "SLOW HANDLER: {RequestName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                    requestName, sw.ElapsedMilliseconds, SlowThresholdMs);
            }
            else
            {
                _logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMs}ms",
                    requestName, sw.ElapsedMilliseconds);
            }

            return response;
        }
    }
}