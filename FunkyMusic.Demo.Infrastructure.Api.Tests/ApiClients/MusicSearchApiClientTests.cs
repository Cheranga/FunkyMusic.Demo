using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using FunkyMusic.Demo.Infrastructure.Api.ApiClients;
using FunkyMusic.Demo.Infrastructure.Api.Configs;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Infrastructure.Api.Tests.ApiClients
{
    [Collection(MusicDemoInfrastructureApiTestsCollection.Name)]
    public class MusicSearchApiClientTests
    {
        private readonly TestsInitializer _testsInitializer;
        private MusicSearchConfig _musicSearchConfig;
        private Mock<HttpClientHandler> _httpClientHandler;
        private HttpClient _httpClient;
        private MusicSearchApiClient _service;
        private Result<MusicArtistSearchResponseDto> _operation;
        private Result<MusicArtistRecordSearchResponseDto> _artistRecordsSearchOperation;

        public MusicSearchApiClientTests(TestsInitializer testsInitializer)
        {
            _httpClientHandler = new Mock<HttpClientHandler>();
            _httpClient = new HttpClient(_httpClientHandler.Object);
            _testsInitializer = testsInitializer;
            _musicSearchConfig = new MusicSearchConfig { ApplicationId = "someappid", MinConfidenceForArtistFilter = 80, Url = "https://www.coles.com.au" };

            _service = new MusicSearchApiClient(_httpClient, _musicSearchConfig, Mock.Of<ILogger<MusicSearchApiClient>>());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("blah")]
        public Task SearchingArtistByNameWithInvalidBaseUrl(string baseUrl)
        {
            this.Given(x => GivenConfiguredBaseUrlIsInvalid(baseUrl))
                .When(x => WhenSearchingForArtistByName())
                .Then(x => ThenMustReturnFailureForArtistByNameSearch())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("blah")]
        public Task SearchingArtistRecordsWithInvalidBaseUrl(string baseUrl)
        {
            this.Given(x => GivenConfiguredBaseUrlIsInvalid(baseUrl))
                .When(x => WhenGettingRecordsForArtist())
                .Then(x => ThenMustReturnFailureForArtistRecordsSearch())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.TooManyRequests)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public Task MusicArtistByNameSearchApiReturnsErrorCode(HttpStatusCode statusCode)
        {
            this.Given(x => GivenMusicSearchReturnsUnsuccessfulHttpStatusCode(statusCode))
                .When(x => WhenSearchingForArtistByName())
                .Then(x => ThenMustReturnFailureForArtistByNameSearch())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.TooManyRequests)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public Task MusicArtistRecordingsSearchApiReturnsErrorCode(HttpStatusCode statusCode)
        {
            this.Given(x => GivenMusicSearchReturnsUnsuccessfulHttpStatusCode(statusCode))
                .When(x => WhenGettingRecordsForArtist())
                .Then(x => ThenMustReturnFailureForArtistRecordsSearch())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("")]
        [InlineData("blah blah")]
        public Task MusicArtistByNameSearchReturnsInvalidData(string responseData)
        {
            this.Given(x => GivenMusicSearchReturnInvalidData(responseData))
                .When(x => WhenSearchingForArtistByName())
                .Then(x => ThenMustReturnFailureForArtistByNameSearch())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("")]
        [InlineData("blah blah")]
        public Task MusicArtistRecordsSearchReturnsInvalidData(string responseData)
        {
            this.Given(x => GivenMusicSearchReturnInvalidData(responseData))
                .When(x => WhenGettingRecordsForArtist())
                .Then(x => ThenMustReturnFailureForArtistRecordsSearch())
                .BDDfy();

            return Task.CompletedTask;
        }


        [Fact]
        public Task MusicArtistByNameSearchReturnsValidData()
        {
            this.Given(x => GivenMusicArtistNameSearchReturnsValidData())
                .When(x => WhenSearchingForArtistByName())
                .Then(x => ThenMustReturnSuccessForArtistNameSearch())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task MusicArtistRecordsSearchReturnsValidData()
        {
            this.Given(x => GivenMusicArtistRecordsSearchReturnValidData())
                .When(x => WhenGettingRecordsForArtist())
                .Then(x => ThenMustReturnSuccessForArtistRecordsSearch())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccessForArtistNameSearch()
        {
            _operation.Should().NotBeNull();
            _operation.Status.Should().BeTrue();
            _operation.Data.Should().NotBeNull();
            _operation.Data.Artists.Should().HaveCountGreaterOrEqualTo(0);

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccessForArtistRecordsSearch()
        {
            _artistRecordsSearchOperation.Should().NotBeNull();
            _artistRecordsSearchOperation.Status.Should().BeTrue();
            _artistRecordsSearchOperation.Data.Should().NotBeNull();
            _artistRecordsSearchOperation.Data.Recordings.Should().HaveCountGreaterOrEqualTo(0);

            return Task.CompletedTask;
        }

        private Task GivenMusicArtistNameSearchReturnsValidData()
        {
            var responseContent = JsonConvert.SerializeObject(_testsInitializer.Fixture.Create<MusicArtistSearchResponseDto>());

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            _httpClientHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            return Task.CompletedTask;
        }

        private Task GivenMusicArtistRecordsSearchReturnValidData()
        {
            var responseContent = JsonConvert.SerializeObject(_testsInitializer.Fixture.Create<MusicArtistRecordSearchResponseDto>());

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            _httpClientHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            return Task.CompletedTask;
        }

        private Task GivenMusicSearchReturnInvalidData(string responseData)
        {
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseData)
            };

            _httpClientHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            return Task.CompletedTask;
        }

        private Task GivenMusicSearchReturnsUnsuccessfulHttpStatusCode(HttpStatusCode statusCode)
        {
            var httpResponse = new HttpResponseMessage(statusCode)
            {
                ReasonPhrase = "some error occured"
            };

            _httpClientHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            return Task.CompletedTask;
        }

        private Task ThenMustReturnFailureForArtistByNameSearch()
        {
            _operation.Should().NotBeNull();
            _operation.Status.Should().BeFalse();
            _operation.ErrorCode.Should().BeEquivalentTo(ErrorCodes.MusicSearchError);

            return Task.CompletedTask;
        }

        private Task ThenMustReturnFailureForArtistRecordsSearch()
        {
            _artistRecordsSearchOperation.Should().NotBeNull();
            _artistRecordsSearchOperation.Status.Should().BeFalse();
            _artistRecordsSearchOperation.ErrorCode.Should().BeEquivalentTo(ErrorCodes.MusicSearchError);

            return Task.CompletedTask;
        }

        private async Task WhenSearchingForArtistByName()
        {
            _operation = await _service.SearchArtistsByNameAsync("blah");
        }

        private async Task WhenGettingRecordsForArtist()
        {
            _artistRecordsSearchOperation = await _service.GetRecordsForArtistByIdAsync("blah");
        }

        private Task GivenConfiguredBaseUrlIsInvalid(string baseUrl)
        {
            _musicSearchConfig.Url = baseUrl;
            return Task.CompletedTask;
        }
    }
}
