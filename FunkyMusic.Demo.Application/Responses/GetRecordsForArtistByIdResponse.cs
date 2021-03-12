using System.Collections.Generic;

namespace FunkyMusic.Demo.Application.Responses
{
    public class GetRecordsForArtistByIdResponse
    {
        public List<Record> Records { get; set; }
    }

    public class Record
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ReleaseDate { get; set; }
    }
}