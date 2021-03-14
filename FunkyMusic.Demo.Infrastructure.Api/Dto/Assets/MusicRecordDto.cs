using System.ComponentModel;

namespace FunkyMusic.Demo.Infrastructure.Api.Dto.Assets
{
    public class MusicRecordDto
    {
        public string Title { get; set; }
        [DefaultValue(0)]
        public int? Length { get; set; }
        public string Id { get; set; }
    }
}