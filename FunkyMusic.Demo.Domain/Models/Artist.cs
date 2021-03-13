using System.Diagnostics.CodeAnalysis;

namespace FunkyMusic.Demo.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public class Artist
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ArtistType Type { get; set; }
        public string Country { get; set; }
        public Gender Gender { get; set; }
    }

    public enum ArtistType
    {
        Person = 1,
        Group = 2
    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }
}