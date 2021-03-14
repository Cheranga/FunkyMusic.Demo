using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Api.Functions;
using FunkyMusic.Demo.Api.ResponseFormatters;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Api.Tests.Functions
{
    [Collection(MusicDemoApiTestsCollection.Name)]
    public class GetArtistByNameFunctionTests
    {
        private readonly TestsInitializer _testsInitializer;
        private readonly GetArtistByNameFunction _function;
        private HttpRequest _httpRequest;
        private readonly ILogger<GetArtistByNameFunction> _logger;
        private readonly Mock<IMediator> _mediator;
        private IActionResult _response;

        public GetArtistByNameFunctionTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _mediator = new Mock<IMediator>();
            _logger = Mock.Of<ILogger<GetArtistByNameFunction>>();

            _function = new GetArtistByNameFunction(_mediator.Object, new SearchArtistByNameResponseDtoFormatter(), _logger);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public Task InvalidCorrelationIdInHeader(string correlationId)
        {
            this.Given(x => GivenInvalidCorrelationIdIsProvidedInRequest(correlationId))
                .When(x => WhenSearchByArtistNameTriggers())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.BadRequest))
                .And(x => ThenMustContainErrorsInTheResponse())
                .BDDfy();

            return Task.CompletedTask;
        }


        private Task ThenMustContainErrorsInTheResponse()
        {
            var badRequestResponse = (ObjectResult) _response;
            var errorResponse = (ErrorResponse) badRequestResponse.Value;
            errorResponse.Should().NotBeNull();
            errorResponse.Errors.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }


        private async Task WhenSearchByArtistNameTriggers()
        {
            _response = await _function.SearchAsync(_httpRequest);
        }

        private Task GivenInvalidCorrelationIdIsProvidedInRequest(string correlationId)
        {
            _httpRequest = _testsInitializer.GetMockedRequest(new Dictionary<string, StringValues>
            {
                {"correlationId", correlationId}
            });

            _mediator.Setup(x => x.Send(It.IsAny<SearchArtistByNameRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchArtistByNameResponseDto>.Failure(ErrorCodes.ValidationError, "errormessage"));

            return Task.CompletedTask;
        }

        private Task ThenMustReturnErrorResponse(HttpStatusCode expectedHttpStatusCode)
        {
            var badRequestResponse = (ObjectResult) _response;
            badRequestResponse.Should().NotBeNull();
            badRequestResponse.StatusCode.Should().Be((int) expectedHttpStatusCode);
            return Task.CompletedTask;
        }
    }
}