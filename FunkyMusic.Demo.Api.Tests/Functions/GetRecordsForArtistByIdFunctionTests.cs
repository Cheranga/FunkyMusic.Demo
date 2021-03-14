using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
    public class GetRecordsForArtistByIdFunctionTests
    {
        private readonly TestsInitializer _testsInitializer;
        private readonly GetRecordsForArtistByIdFunction _function;
        private HttpRequest _httpRequest;
        private readonly ILogger<GetRecordsForArtistByIdFunction> _logger;
        private readonly Mock<IMediator> _mediator;
        private IActionResult _response;

        public GetRecordsForArtistByIdFunctionTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _mediator = new Mock<IMediator>();
            _logger = Mock.Of<ILogger<GetRecordsForArtistByIdFunction>>();

            _function = new GetRecordsForArtistByIdFunction(_mediator.Object, new SearchRecordsForArtistByIdResponseDtoFormatter());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public Task InvalidCorrelationIdInHeader(string correlationId)
        {
            this.Given(x => GivenInvalidCorrelationIdIsProvidedInRequest(correlationId))
                .When(x => WhenSearchForRecordsByArtistIdTriggers())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.BadRequest))
                .And(x => ThenMustContainErrorsInTheResponse())
                .BDDfy();

            return Task.CompletedTask;
        }


        private Task ThenMustContainErrorsInTheResponse()
        {
            var badRequestResponse = (ObjectResult)_response;
            var errorResponse = (ErrorResponse)badRequestResponse.Value;
            errorResponse.Should().NotBeNull();
            errorResponse.Errors.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }


        private async Task WhenSearchForRecordsByArtistIdTriggers()
        {
            _response = await _function.SearchAsync(_httpRequest, "someartistid");
        }

        private Task GivenInvalidCorrelationIdIsProvidedInRequest(string correlationId)
        {
            _httpRequest = _testsInitializer.GetMockedRequest(new Dictionary<string, StringValues>
            {
                {"correlationId", correlationId}
            });

            _mediator.Setup(x => x.Send(It.IsAny<SearchRecordsForArtistByIdRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchRecordsForArtistByIdResponseDto>.Failure(ErrorCodes.ValidationError, "errormessage"));

            return Task.CompletedTask;
        }

        private Task ThenMustReturnErrorResponse(HttpStatusCode expectedHttpStatusCode)
        {
            var badRequestResponse = (ObjectResult)_response;
            badRequestResponse.Should().NotBeNull();
            badRequestResponse.StatusCode.Should().Be((int)expectedHttpStatusCode);
            return Task.CompletedTask;
        }
    }
}
