using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FunkyMusic.Demo.Api.Extensions
{
    public static class HttpExtensions
    {
        public static string GetHeaderValue(this HttpRequest request, string headerName)
        {
            if (request.Headers.TryGetValue(headerName, out var headerValueData))
            {
                var headerValue = headerValueData.FirstOrDefault()?.Trim();
                return headerValue;
            }

            return string.Empty;
        }

        public static async Task<TModel> ToModel<TModel>(this HttpRequest request, Action<TModel> setPropertiesAction = null) where TModel : class
        {
            try
            {
                var content = await new StreamReader(request.Body).ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return null;
                }

                var model = JsonConvert.DeserializeObject<TModel>(content);
                if (model != null)
                {
                    setPropertiesAction?.Invoke(model);
                }

                return model;
            }
            catch
            {
                return null;
            }
        }
    }
}