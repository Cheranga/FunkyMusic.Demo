using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using FunkyMusic.Demo.Api.Dto.Assets;
using Newtonsoft.Json;

namespace FunkyMusic.Demo.Api.Dto.Responses
{
    [ExcludeFromCodeCoverage]
    public class SearchArtistByNameResponseDto
    {   
        public List<ArtistDto> Artists { get; set; } = new List<ArtistDto>();
    }
}