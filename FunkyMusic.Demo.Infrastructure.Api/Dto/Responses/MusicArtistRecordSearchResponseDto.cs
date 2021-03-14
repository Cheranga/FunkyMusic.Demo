using System.Collections.Generic;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Assets;

namespace FunkyMusic.Demo.Infrastructure.Api.Dto.Responses
{
    public class MusicArtistRecordSearchResponseDto
    {
        public string ArtistName { get; set; }
        public List<MusicRecordDto> Recordings { get; set; }
    }
}