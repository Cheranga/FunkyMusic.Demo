using FunkyMusic.Demo.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FunkyMusic.Demo.Api.ResponseFormatters
{
    public interface IResponseFormatter<T> where T : class
    {
        IActionResult GetActionResult(Result<T> result);
    }
}