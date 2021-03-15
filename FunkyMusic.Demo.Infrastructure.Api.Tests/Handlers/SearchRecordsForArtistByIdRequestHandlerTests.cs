using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Infrastructure.Api.ApiClients;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Assets;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;
using FunkyMusic.Demo.Infrastructure.Api.Handlers;
using Moq;
using TestStack.BDDfy;
using Xunit;
using Record = FunkyMusic.Demo.Domain.Models.Record;

namespace FunkyMusic.Demo.Infrastructure.Api.Tests.Handlers
{
    [Collection(MusicDemoInfrastructureApiTestsCollection.Name)]
    public class SearchRecordsForArtistByIdRequestHandlerTests
    {
        private readonly TestsInitializer _testsInitializer;

        public SearchRecordsForArtistByIdRequestHandlerTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _request = _testsInitializer.Fixture.Create<SearchRecordsForArtistByIdRequest>();
            _musicSearchClient = new Mock<IMusicSearchApiClient>();
            _handler = new SearchRecordsForArtistByIdRequestHandler(_musicSearchClient.Object);
        }

        private readonly Mock<IMusicSearchApiClient> _musicSearchClient;
        private readonly SearchRecordsForArtistByIdRequestHandler _handler;
        private readonly SearchRecordsForArtistByIdRequest _request;
        private Result<SearchRecordsForArtistByIdResponse> _operation;

        private Task ThenMustReturnFailure()
        {
            _operation.Should().NotBeNull();
            _operation.Status.Should().BeFalse();

            return Task.CompletedTask;
        }

        private async Task WhenHandlerExecutes()
        {
            _operation = await _handler.Handle(_request, CancellationToken.None);
        }

        private Task GivenArtistRecordSearchFails()
        {
            _musicSearchClient.Setup(x => x.GetRecordsForArtistByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<MusicArtistRecordSearchResponseDto>.Failure("errorcode", "errormessage"));

            return Task.CompletedTask;
        }

        [Fact]
        public Task MusicSearchFails()
        {
            this.Given(x => GivenArtistRecordSearchFails())
                .When(x => WhenHandlerExecutes())
                .Then(x => ThenMustReturnFailure())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task NoRecordingsForArtist()
        {
            this.Given(x => GivenThereAreNoRecordsForArtist())
                .When(x => WhenHandlerExecutes())
                .Then(x => ThenMustReturnFailure())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task RecordingsAreFoundForArtist()
        {
            this.Given(x => GivenThereAreRecordsForArtist())
                .When(x => WhenHandlerExecutes())
                .Then(x => ThenMustReturnSuccess())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccess()
        {
            _operation.Should().NotBeNull();
            _operation.Status.Should().BeTrue();

            return Task.CompletedTask;
        }

        private Task GivenThereAreRecordsForArtist()
        {
            _musicSearchClient.Setup(x => x.GetRecordsForArtistByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<MusicArtistRecordSearchResponseDto>.Success(new MusicArtistRecordSearchResponseDto
                {
                    ArtistName = "some artist",
                    Recordings = _testsInitializer.Fixture.CreateMany<MusicRecordDto>().ToList()
                }));

            return Task.CompletedTask;
        }

        private Task GivenThereAreNoRecordsForArtist()
        {
            _musicSearchClient.Setup(x => x.GetRecordsForArtistByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<MusicArtistRecordSearchResponseDto>.Success(new MusicArtistRecordSearchResponseDto
                {
                    ArtistName = "some artist",
                    Recordings = new List<MusicRecordDto>()
                }));

            return Task.CompletedTask;
        }
    }
}