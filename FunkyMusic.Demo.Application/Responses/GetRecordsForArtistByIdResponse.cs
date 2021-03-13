using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FunkyMusic.Demo.Application.Responses
{
    [ExcludeFromCodeCoverage]
    public class GetRecordsForArtistByIdResponse
    {
        public List<Record> Records { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Record
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ReleaseDate { get; set; }
    }
}