using FluentValidation;
using JobMarketplace.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JobMarketplace.Application
{
    /// <summary>
    /// Registers all Application services. Assembly scanning auto-discovers
    /// handlers, validators, and mapping profiles — no manual registration needed.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());           // Finds all Profile classes (MappingProfile)
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // Finds all AbstractValidator<T> classes

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); // Finds all IRequestHandler<,> classes

                // Pipeline: Request → Validation → Logging → Handler
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            });

            return services;
        }
    }
}