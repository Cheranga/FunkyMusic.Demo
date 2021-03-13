using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Api.Handlers;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using MediatR;
using Moq;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Api.Tests.Handlers
{
    [Collection(MusicDemoApiTestsCollection.Name)]
    public class SearchArtistByNameHandlerTests
    {
        private static TestsInitializer _testsInitializer;
        private Mock<IMediator> _mediator;
        private SearchArtistByNameHandler _handler;
        private SearchArtistByNameRequestDto _request;
        private Result<SearchArtistByNameResponseDto> _result;

        public SearchArtistByNameHandlerTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _request = testsInitializer.Fixture.Create<SearchArtistByNameRequestDto>();
            _mediator = new Mock<IMediator>();

            _handler = new SearchArtistByNameHandler(_mediator.Object);
        }

        [Fact]
        public Task ErrorOccursInWhenSearchingForArtistByName()
        {
            this.Given(x => GivenMusicSearchReturnsAnError())
                .When(x => WhenTheHandlerExecutes())
                .Then(x => ThenMustReturnFailure())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [MemberData(nameof(GetNotFoundArtists))]
        public Task ArtistsAreNotFound(List<Artist> artists)
        {
            this.Given(x => GivenMusicSearchReturnsArtists(artists))
                .When(x => WhenTheHandlerExecutes())
                .Then(x => ThenMustReturnFailure())
                .And(x=> ThenMustHaveErrorCode(ErrorCodes.ArtistNotFound))
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [MemberData(nameof(GetFoundArtists))]
        public Task ArtistsAreFound(List<Artist> artists)
        {
            this.Given(x => GivenMusicSearchReturnsArtists(artists))
                .When(x => WhenTheHandlerExecutes())
                .Then(x => ThenMustReturnSuccess())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccess()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeTrue();

            return Task.CompletedTask;
        }

        private Task ThenMustHaveErrorCode(string expectedErrorCode)
        {
            _result.ErrorCode.Should().BeEquivalentTo(expectedErrorCode);

            return Task.CompletedTask;
        }

        private Task GivenMusicSearchReturnsArtists(List<Artist> artists)
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetArtistByNameRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetArtistByNameResponse>.Success(new GetArtistByNameResponse {Artists = artists}));

            return Task.CompletedTask;
        }

        public static IEnumerable<object[]> GetNotFoundArtists()
        {
            var artists = new List<object[]>
            {
                new object[]{null},
                new object[]{new List<Artist>() }
            };

            return artists;
        }

        public static IEnumerable<object[]> GetFoundArtists()
        {
            var artists = new List<object[]>
            {
                new object[]{new List<Artist>(new []{ _testsInitializer.Fixture.Create<Artist>() }), },
                new object[]{new List<Artist>(_testsInitializer.Fixture.CreateMany<Artist>()) }
            };

            return artists;
        }

        private Task ThenMustReturnFailure()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeFalse();

            return Task.CompletedTask;
        }

        private async Task WhenTheHandlerExecutes()
        {
            _result = await _handler.Handle(_request, CancellationToken.None);
        }

        private Task GivenMusicSearchReturnsAnError()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetArtistByNameRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetArtistByNameResponse>.Failure("errorcode", "errormessage"));

            return Task.CompletedTask;
        }
    }
}
