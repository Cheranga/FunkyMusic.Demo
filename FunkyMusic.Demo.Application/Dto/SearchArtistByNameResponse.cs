using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Domain.Models;

namespace FunkyMusic.Demo.Application.Dto
{
    [ExcludeFromCodeCoverage]
    public class SearchArtistByNameResponse
    {
        public List<Artist> Artists { get; set; } = new List<Artist>();
    }
}