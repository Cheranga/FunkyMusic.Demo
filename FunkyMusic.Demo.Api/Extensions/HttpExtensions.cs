using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FunkyMusic.Demo.Api.Extensions
{
    public static class HttpExtensions
    {
        public static string GetHeaderValue(this HttpRequest request, string headerName)
        {
            if (request?.Headers == null)
            {
                return string.Empty;
            }

            if (request.Headers.TryGetValue(headerName, out var headerValueData))
            {
                var headerValue = headerValueData.FirstOrDefault()?.Trim();
                return headerValue;
            }

            return string.Empty;
        }

        public static string GetQueryStringValue(this HttpRequest request, string queryParameterName)
        {
            if (request?.Query == null)
            {
                return string.Empty;
            }

            if (request.Query.TryGetValue(queryParameterName, out var queryParameterValueData))
            {
                var queryParameterValue = queryParameterValueData.FirstOrDefault()?.Trim();
                return queryParameterValue;
            }

            return string.Empty;
        }
    }
}