using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace FunkyMusic.Demo.Infrastructure.Api.Dto.Assets
{
    [ExcludeFromCodeCoverage]
    public class MusicRecordDto
    {
        public string Title { get; set; }
        [DefaultValue(0)]
        public int? Length { get; set; }
        public string Id { get; set; }
    }
}