using System;
using System.Collections.Generic;
using System.Text;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Assets;

namespace FunkyMusic.Demo.Infrastructure.Api.Dto.Responses
{
    public class MusicArtistSearchResponseDto
    {
        public List<MusicArtistDto> Artists { get; set; } = new List<MusicArtistDto>();
    }
}
