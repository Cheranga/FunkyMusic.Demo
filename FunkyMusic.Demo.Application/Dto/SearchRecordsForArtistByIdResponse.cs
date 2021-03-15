using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Domain.Models;

namespace FunkyMusic.Demo.Application.Dto
{
    [ExcludeFromCodeCoverage]
    public class SearchRecordsForArtistByIdResponse
    {
        public List<Record> Records { get; set; } = new List<Record>();
    }
}