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
    public class SearchArtistByNameRequestDtoValidatorTests
    {
        private SearchArtistByNameRequestDtoValidator _validator;
        private SearchArtistByNameRequestDto _request;
        private ValidationResult _validationResult;

        public SearchArtistByNameRequestDtoValidatorTests(TestsInitializer testsInitializer)
        {
            _validator = new SearchArtistByNameRequestDtoValidator();
            _request = testsInitializer.Fixture.Create<SearchArtistByNameRequestDto>();
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
        public void InvalidArtistName(string artistName)
        {
            this.Given(x => GivenInvalidArtistName(artistName))
                .When(x => WhenValidationIsPerformed())
                .Then(x => ThenValidationMustFailForArtistName())
                .BDDfy();
        }

        private void ThenValidationMustFailForArtistName()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validator.ShouldHaveValidationErrorFor(x=> x.ArtistName, _request);
        }

        private void GivenInvalidArtistName(string artistName)
        {
            _request.ArtistName = artistName;
        }

        private void ThenValidationMustFailForCorrelationId()
        {
            _validationResult.Should().NotBeNull();
            _validator.ShouldHaveValidationErrorFor(x => x.CorrelationId, _request);
        }

        private void WhenValidationIsPerformed()
        {
            _validationResult = _validator.Validate(_request);
        }

        private void GivenInvalidCorrelationId(string correlationId)
        {
            _request.CorrelationId = correlationId;
        }
    }
}
