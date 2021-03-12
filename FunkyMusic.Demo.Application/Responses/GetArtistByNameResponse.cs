using System.Collections.Generic;

namespace FunkyMusic.Demo.Application.Responses
{
    public class GetArtistByNameResponse
    {
        public List<Artist> Artists { get; set; } = new List<Artist>();
    }

    public class Artist
    {
        public string ArtistId { get; set; }
        public string ArtistName { get; set; }
    }
}