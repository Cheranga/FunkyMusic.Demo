using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Api.Dto.Assets;

namespace FunkyMusic.Demo.Api.Dto.Responses
{
    [ExcludeFromCodeCoverage]
    public class SearchRecordsForArtistByIdResponseDto
    {
        public List<RecordDto> Records { get; set; } = new List<RecordDto>();
    }
}