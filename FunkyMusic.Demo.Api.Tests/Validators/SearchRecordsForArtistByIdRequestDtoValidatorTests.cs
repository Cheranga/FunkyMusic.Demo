using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Validators;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Api.Tests.Validators
{
    [Collection(MusicDemoApiTestsCollection.Name)]
    public class SearchRecordsForArtistByIdRequestDtoValidatorTests
    {
        private SearchRecordsForArtistByIdRequestDtoValidator _validator;
        private SearchRecordsForArtistByIdRequestDto _request;
        private ValidationResult _validationResult;

        public SearchRecordsForArtistByIdRequestDtoValidatorTests(TestsInitializer testsInitializer)
        {
            _validator = new SearchRecordsForArtistByIdRequestDtoValidator();
            _request = testsInitializer.Fixture.Create<SearchRecordsForArtistByIdRequestDto>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void InvalidCorrelationId(string correlationId)
        {
            this.Given(x => GivenInvalidCorrelationId(correlationId))
                .When(x => WhenValidationIsPerformed())
                .Then(x => ThenValidationMustFailForCorrelationId())
                .BDDfy();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void InvalidArtistId(string artistId)
        {
            this.Given(x => GivenInvalidArtistId(artistId))
                .When(x => WhenValidationIsPerformed())
                .Then(x => ThenValidationMustFailForArtistId())
                .BDDfy();
        }

        private void ThenValidationMustFailForArtistId()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validator.ShouldHaveValidationErrorFor(x => x.ArtistId, _request);
        }

        private void GivenInvalidArtistId(string artistId)
        {
            _request.ArtistId = artistId;
        }

        private void GivenInvalidCorrelationId(string correlationId)
        {
            _request.CorrelationId = correlationId;
        }

        private void WhenValidationIsPerformed()
        {
            _validationResult = _validator.Validate(_request);
        }

        private void ThenValidationMustFailForCorrelationId()
        {
            _validationResult.Should().NotBeNull();
            _validator.ShouldHaveValidationErrorFor(x => x.CorrelationId, _request);
        }


    }
}
