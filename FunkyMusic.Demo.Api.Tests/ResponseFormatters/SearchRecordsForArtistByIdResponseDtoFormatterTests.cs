using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class SearchRecordsForArtistByIdResponseDtoFormatterTests
    {
        private readonly TestsInitializer _testsInitializer;

        public SearchRecordsForArtistByIdResponseDtoFormatterTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _responseFormatter = new SearchRecordsForArtistByIdResponseDtoFormatter();
        }

        private Result<SearchRecordsForArtistByIdResponseDto> _result;
        private readonly SearchRecordsForArtistByIdResponseDtoFormatter _responseFormatter;
        private IActionResult _response;

        private void ThenMustContainValidationErrors()
        {
            var badRequestResponse = (ObjectResult) _response;
            var errorResponse = (ErrorResponse) badRequestResponse.Value;
            errorResponse.Should().NotBeNull();
            errorResponse.Errors.Should().NotBeNullOrEmpty();
        }

        private void ThenMustReturnErrorResponse(HttpStatusCode expectedHttpStatusCode)
        {
            var badRequestResponse = (ObjectResult) _response;
            badRequestResponse.Should().NotBeNull();
            badRequestResponse.StatusCode.Should().Be((int) expectedHttpStatusCode);
        }

        private void WhenResponseIsReturned()
        {
            _response = _responseFormatter.GetActionResult(_result);
        }

        private void GivenResultIsNull()
        {
            _result = null;
        }

        [Fact]
        public void ResultIsNull()
        {
            this.Given(x => GivenResultIsNull())
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.InternalServerError))
                .Then(x => ThenMustContainValidationErrors())
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
                .And(x => ThenMustContainValidationErrors())
                .BDDfy();
        }

        [Fact]
        public void ArtistRecordsSearchError()
        {
            this.Given(x => GivenThereWasAnErrorWhenSearchingRecordsForArtist())
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.InternalServerError))
                .And(x => ThenMustContainValidationErrors())
                .BDDfy();
        }

        [Fact]
        public void ArtistRecordsNotFound()
        {
            this.Given(x => GivenArtistRecordsWereNotFound())
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.NotFound))
                .Then(x => ThenMustContainValidationErrors())
                .BDDfy();
        }

        [Fact]
        public void ArtistRecordsAreFound()
        {
            this.Given(x => GivenArtistRecordsWereFound())
                .When(x => WhenResponseIsReturned())
                .Then(x => ThenMustContainRecordsData())
                .BDDfy();
        }

        private void ThenMustContainRecordsData()
        {
            var objectResult = (ObjectResult)(_response);
            objectResult.Should().NotBeNull();

            var artists = (SearchRecordsForArtistByIdResponseDto)(objectResult.Value);
            artists.Should().NotBeNull();
            artists.Records.Should().HaveCountGreaterThan(1);
        }

        private void GivenArtistRecordsWereFound()
        {
            var response = new SearchRecordsForArtistByIdResponseDto
            {
                Records = new List<RecordDto>(_testsInitializer.Fixture.CreateMany<RecordDto>().ToList())
            };

            _result = Result<SearchRecordsForArtistByIdResponseDto>.Success(response);
        }

        private void GivenArtistRecordsWereNotFound()
        {
            _result = Result<SearchRecordsForArtistByIdResponseDto>.Failure(ErrorCodes.ArtistRecordsNotFound, "error messages");

        }

        private void GivenThereWasAnErrorWhenSearchingRecordsForArtist()
        {
            _result = Result<SearchRecordsForArtistByIdResponseDto>.Failure(ErrorCodes.ArtistRecordsSearchError, "error messages");

        }

        private void GivenInvalidCorrelationIdIsInRequest(string correlationId)
        {
            _result = Result<SearchRecordsForArtistByIdResponseDto>.Failure(ErrorCodes.ValidationError, "error messages");

        }
    }
}