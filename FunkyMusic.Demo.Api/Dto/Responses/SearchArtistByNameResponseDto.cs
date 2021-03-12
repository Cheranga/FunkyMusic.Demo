using System.Collections.Generic;
using FunkyMusic.Demo.Api.Dto.Assets;

namespace FunkyMusic.Demo.Api.Dto.Responses
{
    public class SearchArtistByNameResponseDto
    {
        public List<ArtistDto> Artists { get; set; } = new List<ArtistDto>();
    }
}