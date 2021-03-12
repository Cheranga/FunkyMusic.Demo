using System.Collections.Generic;
using FunkyMusic.Demo.Api.Dto.Assets;

namespace FunkyMusic.Demo.Api.Dto.Responses
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public List<ErrorMessage> Errors { get; set; } = new List<ErrorMessage>();
    }
}