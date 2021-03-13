using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Api.Dto.Assets;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Api.ResponseFormatters;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Api.Tests.ResponseFormatters
{
    [Collection(MusicDemoApiTestsCollection.Name)]
    public class GetArtistByNameResponseFormatterTests
    {
        private readonly TestsInitializer _testsInitializer;
        private GetArtistByNameResponseFormatter _responseFormatter;
        private Result<SearchArtistByNameResponseDto> _result;
        private IActionResult _response;

        public GetArtistByNameResponseFormatterTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _responseFormatter = new GetArtistByNameResponseFormatter();
        }

        [Fact]
        public void SingleArtistFoundAfterSearch()
        {
            this.Given(x => GivenOnlyOneArtistWasFound())
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustContainOnlyOneArtistData())
                .BDDfy();
        }

        [Fact]
        public void ManyArtistsAreFoundAfterSearch()
        {
            this.Given(x => GivenManyArtistsAreFound())
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustContainMultipleArtistsData())
                .BDDfy();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ValidationErrorInSearchRequest(string correlationId)
        {
            this.Given(x => GivenInvalidCorrelationIdIsInRequest(correlationId))
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.BadRequest))
                .And(x=> ThenMustContainValidationErrors())
                .BDDfy();
        }

        [Fact]
        public void ArtistNotFound()
        {
            this.Given(x => GivenArtistWasNotFound())
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.NotFound))
                .Then(x => ThenMustContainValidationErrors())
                .BDDfy();
        }

        [Fact]
        public void ArtistSearchError()
        {
            this.Given(x => GivenThereWasAnErrorWhenSearchingForArtist())
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.InternalServerError))
                .And(x => ThenMustContainValidationErrors())
                .BDDfy();
        }

        private void GivenThereWasAnErrorWhenSearchingForArtist()
        {
            _result = Result<SearchArtistByNameResponseDto>.Failure(ErrorCodes.ArtistSearchError, "error messages");
        }

        private void GivenArtistWasNotFound()
        {
            _result = Result<SearchArtistByNameResponseDto>.Failure(ErrorCodes.ArtistNotFound, "error messages");
        }

        private void ThenMustContainValidationErrors()
        {
            var badRequestResponse = (ObjectResult)(_response);
            var errorResponse = (ErrorResponse)(badRequestResponse.Value);
            errorResponse.Should().NotBeNull();
            errorResponse.Errors.Should().NotBeNullOrEmpty();
        }

        private void ThenMustReturnErrorResponse(HttpStatusCode expectedHttpStatusCode)
        {
            var badRequestResponse = (ObjectResult)(_response);
            badRequestResponse.Should().NotBeNull();
            badRequestResponse.StatusCode.Should().Be((int)(expectedHttpStatusCode));
        }

        private void GivenInvalidCorrelationIdIsInRequest(string correlationId)
        {
            _result = Result<SearchArtistByNameResponseDto>.Failure(ErrorCodes.ValidationError, "error messages");
        }

        private void ThenMustContainMultipleArtistsData()
        {
            var objectResult = (ObjectResult)(_response);
            objectResult.Should().NotBeNull();

            var artists = (SearchArtistByNameResponseDto)(objectResult.Value);
            artists.Should().NotBeNull();
            artists.Artists.Should().HaveCountGreaterThan(1);
        }

        private void GivenManyArtistsAreFound()
        {
            var response = new SearchArtistByNameResponseDto
            {
                Artists = new List<ArtistDto>(_testsInitializer.Fixture.CreateMany<ArtistDto>().ToList())
            };

            _result = Result<SearchArtistByNameResponseDto>.Success(response);
        }

        private void ThenMustContainOnlyOneArtistData()
        {
            var objectResult = (ObjectResult) (_response);
            objectResult.Should().NotBeNull();

            var artists = (SearchArtistByNameResponseDto) (objectResult.Value);
            artists.Should().NotBeNull();
            artists.Artists.Should().HaveCount(1);
        }

        private void WhenResponseIsReturned()
        {
            _response = _responseFormatter.GetActionResult(_result);
        }

        private void GivenOnlyOneArtistWasFound()
        {
            var response = new SearchArtistByNameResponseDto
            {
                Artists = new List<ArtistDto>(new[] {_testsInitializer.Fixture.Create<ArtistDto>()})
            };

            _result = Result<SearchArtistByNameResponseDto>.Success(response);
        }
    }
}
