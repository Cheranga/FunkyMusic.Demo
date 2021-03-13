using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Api.Dto.Assets;

namespace FunkyMusic.Demo.Api.Dto.Responses
{
    [ExcludeFromCodeCoverage]
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public List<ErrorMessage> Errors { get; set; } = new List<ErrorMessage>();
    }
}