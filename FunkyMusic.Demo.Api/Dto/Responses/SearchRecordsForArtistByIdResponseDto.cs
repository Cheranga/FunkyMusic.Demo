using System.Collections.Generic;
using FunkyMusic.Demo.Api.Dto.Assets;

namespace FunkyMusic.Demo.Api.Dto.Responses
{
    public class SearchRecordsForArtistByIdResponseDto
    {
        public List<RecordDto> Records { get; set; } = new List<RecordDto>();
    }
}