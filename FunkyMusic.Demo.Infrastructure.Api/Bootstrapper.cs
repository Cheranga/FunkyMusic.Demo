using System;
using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Infrastructure.Api.ApiClients;
using FunkyMusic.Demo.Infrastructure.Api.Configs;
using FunkyMusic.Demo.Infrastructure.Api.Handlers;
using FunkyMusic.Demo.Infrastructure.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FunkyMusic.Demo.Infrastructure.Api
{
    [ExcludeFromCodeCoverage]
    public static class Bootstrapper
    {
        public static void UseExternalMusicSearchApi(this IServiceCollection services, IConfigurationRoot configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<MusicSearchConfig>(configuration.GetSection($"Values:{nameof(MusicSearchConfig)}"));
            services.AddScoped(provider =>
            {
                var config = provider.GetRequiredService<IOptionsSnapshot<MusicSearchConfig>>().Value;
                return config;
            });


            services.AddHttpClient<IMusicSearchApiClient, MusicSearchApiClient>();
            services.AddScoped<IMusicArtistFilterService, MusicArtistFilterService>();
        }
    }
}