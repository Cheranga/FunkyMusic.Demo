using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FunkyMusic.Demo.Api;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Api.ResponseFormatters;
using FunkyMusic.Demo.Application;
using FunkyMusic.Demo.Application.Responses;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FunkyMusic.Demo.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            RegisterMediatr(services);
            RegisterValidators(services);
            RegisterResponseFormatters(services);

            services.UseFunkyApplication();
            Domain.Bootstrapper.UseDomain(services);
            Infrastructure.Api.Bootstrapper.UseExternalMusicSearchApi(services, GetConfigurationRoot(builder));

        }

        private void RegisterResponseFormatters(IServiceCollection services)
        {
            services.AddScoped<IResponseFormatter<SearchArtistByNameResponseDto>, SearchArtistByNameResponseDtoFormatter>();
            services.AddScoped<IResponseFormatter<SearchRecordsForArtistByIdResponseDto>, SearchRecordsForArtistByIdResponseDtoFormatter>();
        }

        private void RegisterValidators(IServiceCollection services)
        {
            var validatorAssemblies = new[] { typeof(Startup).Assembly, typeof(Bootstrapper).Assembly, typeof(Domain.Bootstrapper).Assembly, typeof(Infrastructure.Api.Bootstrapper).Assembly };

            services.AddValidatorsFromAssemblies(validatorAssemblies);
        }

        protected virtual void RegisterMediatr(IServiceCollection services)
        {
            var mediatrAssemblies = new[] { typeof(Startup).Assembly, typeof(Bootstrapper).Assembly, typeof(Domain.Bootstrapper).Assembly, typeof(Infrastructure.Api.Bootstrapper).Assembly };

            services.AddMediatR(mediatrAssemblies);
        }

        protected virtual IConfigurationRoot GetConfigurationRoot(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var executionContextOptions = services.BuildServiceProvider().GetService<IOptions<ExecutionContextOptions>>().Value;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(executionContextOptions.AppDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            return configuration;
        }
    }
}