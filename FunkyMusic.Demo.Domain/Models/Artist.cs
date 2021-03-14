using System.Diagnostics.CodeAnalysis;

namespace FunkyMusic.Demo.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public class Artist
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}