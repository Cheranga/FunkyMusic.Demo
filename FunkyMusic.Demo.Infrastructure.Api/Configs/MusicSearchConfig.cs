namespace FunkyMusic.Demo.Infrastructure.Api.Configs
{
    public class MusicSearchConfig
    {
        public string Url { get; set; }
        public string ApplicationId { get; set; }
        public int MinScoreForArtistFilter { get; set; } = 100;
    }
}