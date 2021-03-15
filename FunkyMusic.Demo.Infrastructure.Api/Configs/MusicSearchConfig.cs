using System.Diagnostics.CodeAnalysis;

namespace FunkyMusic.Demo.Infrastructure.Api.Configs
{
    [ExcludeFromCodeCoverage]
    public class MusicSearchConfig
    {
        public string Url { get; set; }
        public string ApplicationId { get; set; }
        public int MinConfidenceForArtistFilter { get; set; } = 100;
    }
}