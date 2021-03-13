using System.Diagnostics.CodeAnalysis;

namespace FunkyMusic.Demo.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public class Record
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Length { get; set; }
        public string ReleaseDate { get; set; }
    }
}