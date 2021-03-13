using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FunkyMusic.Demo.Application.Tests;
using FunkyMusic.Demo.Domain.Behaviours;
using FunkyMusic.Demo.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Domain.Tests
{
    public class TestResponse
    {
    }

    public class TestRequest : IRequest<TestResponse>, IValidatable
    {
        public string CorrelationId { get; set; }
    }

    [Collection(MusicDemoDomainTestsCollection.Name)]
    public class ValidationBehaviourTests
    {
        public ValidationBehaviourTests(TestsInitializer testsInitializer)
        {
            _validator = new Mock<IValidator<TestRequest>>();
            _logger = Mock.Of<ILogger<ValidationBehaviour<TestRequest, Result<TestResponse>>>>();

            _nextDelegate = new Mock<RequestHandlerDelegate<Result<Result<TestResponse>>>>();

            _request = testsInitializer.Fixture.Create<TestRequest>();
        }

        private readonly Mock<IValidator<TestRequest>> _validator;
        private readonly ILogger<ValidationBehaviour<TestRequest, Result<TestResponse>>> _logger;
        private ValidationBehaviour<TestRequest, Result<TestResponse>> _behaviour;
        private readonly TestRequest _request;
        private Result<Result<TestResponse>> _result;
        private readonly Mock<RequestHandlerDelegate<Result<Result<TestResponse>>>> _nextDelegate;

        private Task GivenValidationWasSuccessful()
        {
            _validator.Setup(x => x.ValidateAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _behaviour = new ValidationBehaviour<TestRequest, Result<TestResponse>>(_validator.Object, _logger);

            return Task.CompletedTask;
        }

        private Task ThenMustReturnFailureWithErrorCode(string expectedErrorCode)
        {
            _result.Status.Should().BeFalse();
            _result.ErrorCode.Should().BeEquivalentTo(expectedErrorCode);

            return Task.CompletedTask;
        }

        private Task ThenWillNotContinuePipelineExecution()
        {
            _nextDelegate.Verify(x => x.Invoke(), Times.Never);
            return Task.CompletedTask;
        }

        private Task GivenValidationFailed()
        {
            _validator.Setup(x => x.ValidateAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(new[] {new ValidationFailure("errorcode", "errormessage")}));
            _behaviour = new ValidationBehaviour<TestRequest, Result<TestResponse>>(_validator.Object, _logger);

            return Task.CompletedTask;
        }

        private Task ThenMustContinuePipelineExecution()
        {
            _nextDelegate.Verify(x => x.Invoke(), Times.Once);
            return Task.CompletedTask;
        }

        private async Task WhenPipelineIsExecuted()
        {
            _result = await _behaviour.Handle(_request, CancellationToken.None, _nextDelegate.Object);
        }


        private Task GivenValidatorIsNull()
        {
            _behaviour = new ValidationBehaviour<TestRequest, Result<TestResponse>>(null, _logger);
            return Task.CompletedTask;
        }

        [Fact]
        public Task ValidationFailed()
        {
            this.Given(x => GivenValidationFailed())
                .When(x => WhenPipelineIsExecuted())
                .Then(x => ThenWillNotContinuePipelineExecution())
                .And(x => ThenMustReturnFailureWithErrorCode(ErrorCodes.ValidationError))
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task ValidationSuccessful()
        {
            this.Given(x => GivenValidationWasSuccessful())
                .When(x => WhenPipelineIsExecuted())
                .Then(x => ThenMustContinuePipelineExecution())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task ValidatorIsNull()
        {
            this.Given(x => GivenValidatorIsNull())
                .When(x => WhenPipelineIsExecuted())
                .Then(x => ThenMustContinuePipelineExecution())
                .BDDfy();

            return Task.CompletedTask;
        }
    }
}