using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Infrastructure.Api.Configs;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Assets;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;
using FunkyMusic.Demo.Infrastructure.Api.Services;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Infrastructure.Api.Tests.Services
{
    [Collection(MusicDemoInfrastructureApiTestsCollection.Name)]
    public class MusicArtistFilterServiceTests
    {
        private readonly TestsInitializer _testsInitializer;
        private MusicArtistFilterService _service;
        private MusicSearchConfig _musicSearchConfig;
        private Result<MusicArtistSearchResponseDto> _result;

        public MusicArtistFilterServiceTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _musicSearchConfig = new MusicSearchConfig {MinConfidenceForArtistFilter = 80};
            _service = new MusicArtistFilterService(_musicSearchConfig);
        }

        [Fact]
        public void TheArtistSearchResultIsNull()
        {
            this.Given(x => GivenArtistSearchResultIsNull())
                .When(x => WhenFilteredByScore())
                .Then(x => ThenMustReturnNullResult())
                .BDDfy();
        }

        [Fact]
        public void TheArtistSearchResultIsFailed()
        {
            this.Given(x => GivenArtistSearchResultIsFailed())
                .When(x => WhenFilteredByScore())
                .Then(x => ThenMustReturnSameFailedResult())
                .BDDfy();
        }

        [Fact]
        public void ThereAreNoArtistsToFilter()
        {
            this.Given(x => GivenThereAreNoArtistsToBeFiltered())
                .When(x => WhenFilteredByScore())
                .Then(x => ThenMustReturnSameEmptyResult())
                .BDDfy();
        }

        [Fact]
        public void ThereAreArtistsToFilter()
        {
            this.Given(x => GivenThereAreArtistsToBeFiltered())
                .And(x=> GivenSomeArtistHaveConfidenceLevelOverOrEqualThan(80))
                .And(x=> GivenThereAreArtistsWhoHaveConfidenceLevelOf(50))
                .When(x => WhenFilteredByScore())
                .Then(x => ThenMustReturnArtistsWhoHaveConfidenceOfMoreThanOrEqualOf(80))
                .BDDfy();
        }

        private void ThenMustReturnArtistsWhoHaveConfidenceOfMoreThanOrEqualOf(int confidence)
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeTrue();
            _result.Data.Artists.TrueForAll(x => x.Score >= confidence);
        }

        private void GivenThereAreArtistsWhoHaveConfidenceLevelOf(int confidence)
        {
            _result.Data.Artists.Last().Score = confidence;
        }

        private void GivenSomeArtistHaveConfidenceLevelOverOrEqualThan(int confidenceLevel)
        {
            _result.Data.Artists[0].Score = confidenceLevel;
            _result.Data.Artists[1].Score = (confidenceLevel + 1);
        }

        private void GivenThereAreArtistsToBeFiltered()
        {
            _result = Result<MusicArtistSearchResponseDto>.Success(new MusicArtistSearchResponseDto
            {
                Artists = _testsInitializer.Fixture.CreateMany<MusicArtistDto>().ToList()
            });
        }

        private void ThenMustReturnSameEmptyResult()
        {
            _result.Should().NotBeNull();
            _result.Data.Should().NotBeNull();
            _result.Data.Artists.Should().BeEmpty();
        }

        private void GivenThereAreNoArtistsToBeFiltered()
        {
            _result = Result<MusicArtistSearchResponseDto>.Success(new MusicArtistSearchResponseDto
            {
                Artists = new List<MusicArtistDto>()
            });
        }

        private void ThenMustReturnSameFailedResult()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeFalse();
        }

        private void GivenArtistSearchResultIsFailed()
        {
            _result = Result<MusicArtistSearchResponseDto>.Failure("errorcode", "errormessage");
        }

        private void ThenMustReturnNullResult()
        {
            _result.Should().BeNull();
        }

        private void WhenFilteredByScore()
        {
            _result = _service.FilterByScore(_result);
        }

        private void GivenArtistSearchResultIsNull()
        {
            _result = null;
        }
    }
}
