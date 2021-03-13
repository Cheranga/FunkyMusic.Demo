using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Api.Dto.Assets;

namespace FunkyMusic.Demo.Api.BFF.Responses
{
    [ExcludeFromCodeCoverage]
    public class SearchArtistByNameResponse
    {
        public string ArtistId { get; set; }
        public string ArtistName { get; set; }
        public List<RecordDto> Records { get; set; } = new List<RecordDto>();
    }
}