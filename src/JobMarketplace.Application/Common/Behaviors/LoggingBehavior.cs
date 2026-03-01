using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior — auto-logs every request and response.
    /// Applies to all commands and queries. No log lines needed in handlers.
    /// Pipeline: Request → ValidationBehavior → [LoggingBehavior] → Handler
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

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
            var response = await next();
            _logger.LogInformation("Handled {RequestName} | {@Response}", requestName, response);

            return response;
        }
    }
}