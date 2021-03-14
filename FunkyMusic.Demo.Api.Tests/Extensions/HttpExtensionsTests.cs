using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FunkyMusic.Demo.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Api.Tests.Extensions
{
    [Collection(MusicDemoApiTestsCollection.Name)]
    public class HttpExtensionsTests
    {
        private readonly TestsInitializer _testsInitializer;
        private static string _headerValue;
        private static string _queryValue;
        private HttpRequest _request;

        public HttpExtensionsTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
        }
        

        [Fact]
        public void HttpRequestIsNull()
        {
            this.Given(x => GivenHttpRequestIsNull())
                .When(x => WhenHeaderDataIsRetrieved("header"))
                .Then(x => ThenMustReturnHeaderData(""))
                .BDDfy();
        }

        [Fact]
        public void HttpHeadersAreNull()
        {
            this.Given(x => GivenHttpHeadersAreNull())
                .When(x => WhenHeaderDataIsRetrieved("header"))
                .Then(x => ThenMustReturnHeaderData(""))
                .BDDfy();
        }

        [Fact]
        public void HttpQueryStringIsNull()
        {
            this.Given(x => GivenHttpQueryStringIsNull())
                .When(x => WhenQueryDataIsRetrieved("name"))
                .Then(x => ThenMustReturnQueryData(""))
                .BDDfy();
        }

        private void ThenMustReturnQueryData(string expectedQueryValue)
        {
            _queryValue.Should().Be(expectedQueryValue);
        }

        private void WhenQueryDataIsRetrieved(string name)
        {
            _queryValue = HttpExtensions.GetQueryStringValue(_request, name);
        }

        private void GivenHttpQueryStringIsNull()
        {
            _request = _testsInitializer.GetMockedRequest();
        }

        [Fact]
        public void HttpHeaderFound()
        {
            this.Given(x => GivenRequiredHttpHeaderIsAvailable())
                .When(x => WhenHeaderDataIsRetrieved("header"))
                .Then(x => ThenMustReturnHeaderData("headervalue"))
                .BDDfy();
        }

        [Fact]
        public void HttpQueryStringFound()
        {
            this.Given(x => GivenRequiredQueryStringIsAvailable())
                .When(x => WhenQueryDataIsRetrieved("name"))
                .Then(x => ThenMustReturnQueryData("cheranga"))
                .BDDfy();
        }

        private void GivenRequiredQueryStringIsAvailable()
        {
            _request = _testsInitializer.GetMockedRequest(null, new Dictionary<string, StringValues>
            {
                {"name", "cheranga"}
            });
        }

        private void GivenRequiredHttpHeaderIsAvailable()
        {
            _request = _testsInitializer.GetMockedRequest(new Dictionary<string, StringValues>
            {
                {"header", "headervalue"}
            });
        }

        [Fact]
        public void HttpHeaderNotFound()
        {
            this.Given(x => GivenRequiredHttpHeaderIsNotAvailable())
                .When(x => WhenHeaderDataIsRetrieved("header"))
                .Then(x => ThenMustReturnHeaderData(""))
                .BDDfy();
        }

        [Fact]
        public void HttpQueryDataNotFound()
        {
            this.Given(x => GivenRequiredQueryDataIsNotAvailable())
                .When(x => WhenQueryDataIsRetrieved("name"))
                .Then(x => ThenMustReturnQueryData(""))
                .BDDfy();
        }

        private void GivenRequiredQueryDataIsNotAvailable()
        {
            _request = _testsInitializer.GetMockedRequest(null, new Dictionary<string, StringValues>
            {
                {"somequery", "somequeryvalue"}
            });
        }

        private void GivenRequiredHttpHeaderIsNotAvailable()
        {
            _request = _testsInitializer.GetMockedRequest(new Dictionary<string, StringValues>
            {
                {"header1", "headervalue"}
            });
        }

        private void GivenHttpHeadersAreNull()
        {
             _request = _testsInitializer.GetMockedRequest();
        }

        private void ThenMustReturnHeaderData(string expectedHeaderValue)
        {
            _headerValue.Should().BeEquivalentTo(expectedHeaderValue);
        }

        private void WhenHeaderDataIsRetrieved(string headerName)
        {
            _headerValue = HttpExtensions.GetHeaderValue(_request, headerName);
        }

        private void GivenHttpRequestIsNull()
        {
            _request = null;
        }
    }
}
