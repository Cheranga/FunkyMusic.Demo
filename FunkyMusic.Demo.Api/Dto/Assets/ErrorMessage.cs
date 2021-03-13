using System.Diagnostics.CodeAnalysis;

namespace FunkyMusic.Demo.Api.Dto.Assets
{
    [ExcludeFromCodeCoverage]
    public class ErrorMessage
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
}