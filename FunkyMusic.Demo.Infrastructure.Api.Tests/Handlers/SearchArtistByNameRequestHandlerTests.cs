using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using FunkyMusic.Demo.Domain.Models;
using FunkyMusic.Demo.Infrastructure.Api.ApiClients;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Assets;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;
using FunkyMusic.Demo.Infrastructure.Api.Handlers;
using FunkyMusic.Demo.Infrastructure.Api.Services;
using Moq;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Infrastructure.Api.Tests.Handlers
{
    [Collection(MusicDemoInfrastructureApiTestsCollection.Name)]
    public class SearchArtistByNameRequestHandlerTests
    {
        private readonly TestsInitializer _testsInitializer;
        private Mock<IMusicSearchApiClient> _musicSearchClient;
        private Mock<IMusicArtistFilterService> _filterService;
        private SearchArtistByNameRequestHandler _handler;
        private SearchArtistByNameRequest _request;
        private Result<List<Artist>> _operation;

        public SearchArtistByNameRequestHandlerTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _request = testsInitializer.Fixture.Create<SearchArtistByNameRequest>();
            _musicSearchClient = new Mock<IMusicSearchApiClient>();
            _filterService = new Mock<IMusicArtistFilterService>();
            _handler = new SearchArtistByNameRequestHandler(_musicSearchClient.Object, _filterService.Object);
        }

        [Fact]
        public Task MusicSearchFails()
        {
            this.Given(x => GivenArtistSearchFails())
                .When(x => WhenHandlerExecutes())
                .Then(x => ThenMustReturnFailure())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task NoArtistsWereFound()
        {
            this.Given(x => GivenArtistWasNotFound())
                .And(x=> GivenNothingToBeFiltered())
                .When(x => WhenHandlerExecutes())
                .Then(x => ThenMustReturnFailure())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task GivenNothingToBeFiltered()
        {
            _filterService.Setup(x => x.FilterByScore(It.IsAny<Result<MusicArtistSearchResponseDto>>()))
                .Returns(Result<MusicArtistSearchResponseDto>.Success(new MusicArtistSearchResponseDto
                {
                    Artists = new List<MusicArtistDto>()
                }));

            return Task.CompletedTask;
        }

        [Fact]
        public Task ArtistsWereFound()
        {
            this.Given(x => GivenArtistsWereFound())
                .And(x=> GivenArtistSearchConfidenceIsSet())
                .When(x => WhenHandlerExecutes())
                .Then(x => ThenMustReturnSuccess())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccess()
        {
            _operation.Should().NotBeNull();
            _operation.Status.Should().BeTrue();
            _operation.Data.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }

        private Task GivenArtistsWereFound()
        {
            _musicSearchClient.Setup(x => x.SearchArtistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<MusicArtistSearchResponseDto>.Success(new MusicArtistSearchResponseDto
                {
                    Artists = _testsInitializer.Fixture.CreateMany<MusicArtistDto>().ToList()
                }));

            return Task.CompletedTask;
        }

        private Task GivenArtistWasNotFound()
        {
            _musicSearchClient.Setup(x => x.SearchArtistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<MusicArtistSearchResponseDto>.Success(new MusicArtistSearchResponseDto
                {
                    Artists = new List<MusicArtistDto>()
                }));

            return Task.CompletedTask;
        }

        [Fact]
        public Task OnlyArtistWhichAreGreaterThanOrEqualTheSearchConfidenceLevelMustBeReturned()
        {
            this.Given(x => GivenArtistsAreFoundWhenSearched())
                .And(x=> GivenArtistSearchConfidenceIsSet())
                .When(x => WhenHandlerExecutes())
                .Then(x => ThenOnlyArtistsWhichAreMostlyConfidentOnMustBeReturned())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task GivenArtistSearchConfidenceIsSet()
        {
            var artists = _testsInitializer.Fixture.CreateMany<MusicArtistDto>(1).ToList();

            _filterService.Setup(x => x.FilterByScore(It.IsAny<Result<MusicArtistSearchResponseDto>>()))
                .Returns(Result<MusicArtistSearchResponseDto>.Success(new MusicArtistSearchResponseDto
                {
                    Artists = artists
                }));

            return Task.CompletedTask;
        }

       

        private Task ThenOnlyArtistsWhichAreMostlyConfidentOnMustBeReturned()
        {
            _operation.Should().NotBeNull();
            _operation.Status.Should().BeTrue();
            _operation.Data.Should().HaveCountGreaterOrEqualTo(1);

            return Task.CompletedTask;
        }

        private Task GivenArtistsAreFoundWhenSearched()
        {
            var artists = _testsInitializer.Fixture.CreateMany<MusicArtistDto>().ToList();

            _musicSearchClient.Setup(x => x.SearchArtistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<MusicArtistSearchResponseDto>.Success(new MusicArtistSearchResponseDto
                {
                    Artists = artists
                }));

            return Task.CompletedTask;
        }

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

        private Task GivenArtistSearchFails()
        {
            _musicSearchClient.Setup(x => x.SearchArtistsByNameAsync(It.IsAny<string>())).ReturnsAsync(Result<MusicArtistSearchResponseDto>.Failure("errorcode", "errormessage"));

            return Task.CompletedTask;
        }
    }
}
