using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Validators;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Application.Tests.Validators
{
    [Collection(MusicDemoApplicationTestsCollection.Name)]
    public class SearchArtistByNameRequestValidatorTests
    {
        private SearchArtistByNameRequest _request;
        private SearchArtistByNameRequestValidator _validator;
        private ValidationResult _validationResult;

        public SearchArtistByNameRequestValidatorTests(TestsInitializer testsInitializer)
        {
            _validator = new SearchArtistByNameRequestValidator();
            _request = testsInitializer.Fixture.Create<SearchArtistByNameRequest>();
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
        public Task InvalidArtistName(string artistName)
        {
            this.Given(x => GivenInvalidArtistName(artistName))
                .When(x => WhenValidationIsPerformed())
                .Then(x => ThenValidationMustFailForName())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public void RequestIsNull()
        {
            this.Given(x => GivenRequestIsNull())
                .When(x => WhenValidationIsPerformed())
                .Then(x => ThenValidationMustFail())
                .BDDfy();
        }

        private void ThenValidationMustFail()
        {
            _validationResult.Should().NotBeNull();
            _validationResult.IsValid.Should().BeFalse();
        }

        private void GivenRequestIsNull()
        {
            _request = null;
        }

        private void ThenValidationMustFailForCorrelationId()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validator.ShouldHaveValidationErrorFor(x => x.CorrelationId, _request);
        }

        private void ThenValidationMustFailForName()
        {
            _validationResult.IsValid.Should().BeFalse();
            _validator.ShouldHaveValidationErrorFor(x => x.Name, _request);
        }

        private void WhenValidationIsPerformed()
        {
            _validationResult = _validator.Validate(_request);
        }

        private void GivenInvalidCorrelationId(string correlationId)
        {
            _request.CorrelationId = correlationId;
        }

        private void GivenInvalidArtistName(string artistName)
        {
            _request.Name = artistName;
        }
    }
}