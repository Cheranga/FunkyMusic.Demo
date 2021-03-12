using System.Collections.Generic;
using System.Web.Http;
using FunkyMusic.Demo.Api.Dto.Assets;
using Newtonsoft.Json;

namespace FunkyMusic.Demo.Api.Dto.Responses
{
    public class SearchArtistByNameResponseDto
    {
        [JsonIgnore]
        public string ErrorCode { get; set; }
        public List<ArtistDto> Artists { get; set; } = new List<ArtistDto>();
    }

    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public List<ErrorMessage> Errors { get; set; } = new List<ErrorMessage>();
    }

    public class ErrorMessage
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
}