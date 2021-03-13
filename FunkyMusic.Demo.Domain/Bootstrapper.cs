using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Domain.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FunkyMusic.Demo.Domain
{
    [ExcludeFromCodeCoverage]
    public static class Bootstrapper
    {
        public static void UseDomain(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        }
    }
}