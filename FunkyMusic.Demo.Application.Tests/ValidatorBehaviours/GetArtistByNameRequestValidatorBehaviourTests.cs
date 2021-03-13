using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Application.Validators;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Behaviours;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Application.Tests.ValidatorBehaviours
{
    [Collection(MusicDemoApplicationTestsCollection.Name)]
    public class GetArtistByNameRequestValidatorBehaviourTests
    {
        private GetArtistByNameRequest _request;
        private readonly ValidationBehaviour<GetArtistByNameRequest, Result<GetArtistByNameResponse>> _validationBehaviour;
        private readonly GetArtistByNameRequestValidator _validator;
        private Result<Result<GetArtistByNameResponse>> _result;

        public GetArtistByNameRequestValidatorBehaviourTests(TestsInitializer testsInitializer)
        {
            var logger = Mock.Of<ILogger<ValidationBehaviour<GetArtistByNameRequest, Result<GetArtistByNameResponse>>>>();
            
            _request = testsInitializer.Fixture.Create<GetArtistByNameRequest>();
            _validator = new GetArtistByNameRequestValidator();
            _validationBehaviour = new ValidationBehaviour<GetArtistByNameRequest, Result<GetArtistByNameResponse>>(_validator, logger);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public Task InvalidCorrelationId(string correlationId)
        {
            this.Given(x => GivenInvalidCorrelationId(correlationId))
                .When(x => WhenValidationIsPerformedThroughPipeline())
                .Then(x => ThenValidationPipelineMustFail())
                .And(x => ThenValidationErrorForCorrelationIdMustBePresent())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public Task InvalidArtistName(string artistName)
        {
            this.Given(x => GivenInvalidArtistName(artistName))
                .When(x => WhenValidationIsPerformedThroughPipeline())
                .Then(x => ThenValidationPipelineMustFail())
                .And(x => ThenValidationErrorForNameMustBePresent())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task GetArtistByNameRequestItselfIsNull()
        {
            this.Given(x => GivenRequestIsNull())
                .When(x => WhenValidationIsPerformedThroughPipeline())
                .Then(x => ThenValidationPipelineMustFail())
                .And(x=> ThenThereMustBeValidationErrors())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenThereMustBeValidationErrors()
        {
            _result.Validation.Errors.Should().NotBeNullOrEmpty();
            return Task.CompletedTask;
        }

        private Task GivenRequestIsNull()
        {
            _request = null;
            return Task.CompletedTask;
        }

        private Task ThenValidationErrorForNameMustBePresent()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Name, _request);
            return Task.CompletedTask;
        }

        private Task ThenValidationErrorForCorrelationIdMustBePresent()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.CorrelationId, _request);
            return Task.CompletedTask;
        }

        private Task ThenValidationPipelineMustFail()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeFalse();

            return Task.CompletedTask;
        }

        private async Task WhenValidationIsPerformedThroughPipeline()
        {
            _result = await _validationBehaviour.Handle(_request, CancellationToken.None, Mock.Of<RequestHandlerDelegate<Result<Result<GetArtistByNameResponse>>>>());
        }

        private Task GivenInvalidCorrelationId(string correlationId)
        {
            _request.CorrelationId = correlationId;
            return Task.CompletedTask;
        }

        private Task GivenInvalidArtistName(string artistName)
        {
            _request.Name = artistName;
            return Task.CompletedTask;
        }
    }
}