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
    public class GetRecordsForArtistByIdRequestValidatorBehaviourTests
    {
        private GetRecordsForArtistByIdRequest _request;
        private readonly ValidationBehaviour<GetRecordsForArtistByIdRequest, Result<GetRecordsForArtistByIdResponse>> _validationBehaviour;
        private readonly GetRecordsForArtistByIdRequestValidator _validator;
        private Result<Result<GetRecordsForArtistByIdResponse>> _result;

        public GetRecordsForArtistByIdRequestValidatorBehaviourTests(TestsInitializer testsInitializer)
        {
            var logger = Mock.Of<ILogger<ValidationBehaviour<GetRecordsForArtistByIdRequest, Result<GetRecordsForArtistByIdResponse>>>>();
            
            _request = testsInitializer.Fixture.Create<GetRecordsForArtistByIdRequest>();
            _validator = new GetRecordsForArtistByIdRequestValidator();
            _validationBehaviour = new ValidationBehaviour<GetRecordsForArtistByIdRequest, Result<GetRecordsForArtistByIdResponse>>(_validator, logger);
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
        public Task InvalidArtistId(string artistId)
        {
            this.Given(x => GivenInvalidArtistId(artistId))
                .When(x => WhenValidationIsPerformedThroughPipeline())
                .Then(x => ThenValidationPipelineMustFail())
                .And(x => ThenValidationErrorForArtistIdMustBePresent())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task GetRecordsForArtistByIdRequestItselfIsNull()
        {
            this.Given(x => GivenRequestIsNull())
                .When(x => WhenValidationIsPerformedThroughPipeline())
                .Then(x => ThenValidationPipelineMustFail())
                .And(x => ThenThereMustBeValidationErrors())
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

        private Task ThenValidationErrorForArtistIdMustBePresent()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.ArtistId, _request);
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
            _result = await _validationBehaviour.Handle(_request, CancellationToken.None, Mock.Of<RequestHandlerDelegate<Result<Result<GetRecordsForArtistByIdResponse>>>>());
        }

        private Task GivenInvalidCorrelationId(string correlationId)
        {
            _request.CorrelationId = correlationId;
            return Task.CompletedTask;
        }

        private Task GivenInvalidArtistId(string artistId)
        {
            _request.ArtistId = artistId;
            return Task.CompletedTask;
        }
    }
}
