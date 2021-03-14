using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;

namespace FunkyMusic.Demo.Api.Tests
{
    public class TestsInitializer
    {
        public TestsInitializer()
        {
            Fixture = new Fixture();
        }

        public Fixture Fixture { get; set; }

        public async Task<HttpRequest> GetMockedRequest<TData>(TData data, Dictionary<string, StringValues> headers = null) where TData : class
        {
            var serializedData = data == null ? string.Empty : JsonConvert.SerializeObject(data);
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream) {AutoFlush = true};

            await streamWriter.WriteAsync(serializedData);
            memoryStream.Position = 0;

            var mockedRequest = new Mock<HttpRequest>();
            mockedRequest.Setup(x => x.Body).Returns(memoryStream);

            var httpHeaders = headers ?? new Dictionary<string, StringValues>();
            mockedRequest.Setup(x => x.Headers).Returns(new HeaderDictionary(httpHeaders));

            return mockedRequest.Object;
        }

        public HttpRequest GetMockedRequest(Dictionary<string, StringValues> headers = null, Dictionary<string, StringValues> queries = null)
        {
            var mockedRequest = new Mock<HttpRequest>();

            mockedRequest.Setup(x => x.Headers).Returns(headers == null? null: new HeaderDictionary(headers));
            mockedRequest.Setup(x => x.Query).Returns(queries == null ? null : new QueryCollection(queries));

            return mockedRequest.Object;
        }
    }
}