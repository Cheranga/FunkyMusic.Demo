using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Application.Handlers;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using MediatR;
using Moq;
using TestStack.BDDfy;
using Xunit;
using Artist = FunkyMusic.Demo.Domain.Models.Artist;

namespace FunkyMusic.Demo.Application.Tests.Handlers
{
    [Collection(MusicDemoApplicationTestsCollection.Name)]
    public class GetArtistByNameRequestHandlerTests
    {
        public GetArtistByNameRequestHandlerTests(TestsInitializer testsInitializer)
        {
            _fixture = testsInitializer.Fixture;
            _mediator = new Mock<IMediator>();
            _request = _fixture.Create<GetArtistByNameRequest>();
            _handler = new GetArtistByNameRequestHandler(_mediator.Object);
        }

        private readonly Mock<IMediator> _mediator;
        private readonly GetArtistByNameRequestHandler _handler;
        private readonly GetArtistByNameRequest _request;
        private Result<GetArtistByNameResponse> _result;
        private static Fixture _fixture;

        private Task ThenMustReturnFailure()
        {
            _result.Status.Should().BeFalse();

            return Task.CompletedTask;
        }

        private async Task WhenApplicationRequestHandlerExcutes()
        {
            _result = await _handler.Handle(_request, CancellationToken.None);
        }

        private Task GivenThirdPartyMusicServiceReturnsError()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchArtistByNameRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchArtistByNameResponse>.Failure("errorcode", "errormessage"));

            return Task.CompletedTask;
        }

        private Task ThenMustReturnErrorCode(string expectedErrorCode)
        {
            _result.ErrorCode.Should().BeEquivalentTo(expectedErrorCode);

            return Task.CompletedTask;
        }

        private Task GivenNoArtistsCanBeFoundForSearch(List<Artist> artists)
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchArtistByNameRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchArtistByNameResponse>.Success(new SearchArtistByNameResponse
                {
                    Artists = artists
                }));

            return Task.CompletedTask;
        }

        public static IEnumerable<object[]> GetFailedArtists()
        {
            var artists = new List<object[]>
            {
                new object[] {null},
                new object[] {new List<Artist>()}
            };

            return artists;
        }

        public static IEnumerable<object[]> GetFoundArtists()
        {
            var artists = new List<object[]>
            {
                new object[] {new List<Artist>{ _fixture.Create<Artist>() } },
                new object[] {_fixture.CreateMany<Artist>().ToList()}
            };

            return artists;
        }

        [Theory]
        [MemberData(nameof(GetFailedArtists))]
        public Task ArtistNotFound(List<Artist> artists)
        {
            this.Given(x => GivenNoArtistsCanBeFoundForSearch(artists))
                .When(x => WhenApplicationRequestHandlerExcutes())
                .Then(x => ThenMustReturnFailure())
                .And(x => ThenMustReturnErrorCode(ErrorCodes.ArtistNotFound))
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task MusicSearchFails()
        {
            this.Given(x => GivenThirdPartyMusicServiceReturnsError())
                .When(x => WhenApplicationRequestHandlerExcutes())
                .Then(x => ThenMustReturnFailure())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [MemberData(nameof(GetFoundArtists))]
        public Task ArtistFound(List<Artist> artists)
        {
            this.Given(x => GivenThereAreArtistsMatchingSearch(artists))
                .When(x => WhenApplicationRequestHandlerExcutes())
                .Then(x => ThenMustReturnSuccess())
                .And(x=> ThenResponseMustContainArtists())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenResponseMustContainArtists()
        {
            _result.Data.Should().NotBeNull();
            _result.Data.Artists.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccess()
        {
            _result.Status.Should().BeTrue();

            return Task.CompletedTask;
        }

        private Task GivenThereAreArtistsMatchingSearch(List<Artist> artists)
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchArtistByNameRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchArtistByNameResponse>.Success(new SearchArtistByNameResponse
                {
                    Artists = artists
                }));

            return Task.CompletedTask;
        }
    }
}