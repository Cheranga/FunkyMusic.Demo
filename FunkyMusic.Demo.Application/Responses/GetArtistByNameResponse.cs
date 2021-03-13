using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FunkyMusic.Demo.Application.Responses
{
    [ExcludeFromCodeCoverage]
    public class GetArtistByNameResponse
    {
        public List<Artist> Artists { get; set; } = new List<Artist>();
    }

    [ExcludeFromCodeCoverage]
    public class Artist
    {
        public string ArtistId { get; set; }
        public string ArtistName { get; set; }
    }
}