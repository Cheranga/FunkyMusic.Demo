using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace FunkyMusic.Demo.Application
{
    [ExcludeFromCodeCoverage]
    public static class Bootstrapper
    {
        public static void UseFunkyApplication(this IServiceCollection services)
        {
            // Register dependencies.
        }
    }
}