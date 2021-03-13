using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Validators;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Application.Tests.Validators
{
    [Collection(MusicDemoApplicationTestsCollection.Name)]
    public class GetRecordsForArtistByIdRequestValidatorTests
    {
        private readonly GetRecordsForArtistByIdRequest _request;
        private readonly GetRecordsForArtistByIdRequestValidator _validator;
        private ValidationResult _validationResult;

        public GetRecordsForArtistByIdRequestValidatorTests(TestsInitializer testsInitializer)
        {
            _validator = new GetRecordsForArtistByIdRequestValidator();
            _request = testsInitializer.Fixture.Create<GetRecordsForArtistByIdRequest>();
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

        private void ThenValidationMustFailForCorrelationId()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validator.ShouldHaveValidationErrorFor(x => x.CorrelationId, _request);
        }

        private void ThenValidationMustFailForArtistId()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validator.ShouldHaveValidationErrorFor(x => x.ArtistId, _request);
        }

        private void WhenValidationIsPerformed()
        {
            _validationResult = _validator.Validate(_request);
        }

        private void GivenInvalidCorrelationId(string correlationId)
        {
            _request.CorrelationId = correlationId;
        }

        private void GivenInvalidArtistId(string artistId)
        {
            _request.ArtistId = artistId;
        }
    }
}