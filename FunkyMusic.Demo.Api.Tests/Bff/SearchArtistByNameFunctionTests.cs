using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Api.BFF.Functions;
using FunkyMusic.Demo.Api.BFF.Responses;
using FunkyMusic.Demo.Api.Dto.Assets;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Api.ResponseFormatters;
using FunkyMusic.Demo.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using TestStack.BDDfy;
using Xunit;

namespace FunkyMusic.Demo.Api.Tests.Bff
{
    [Collection(MusicDemoApiTestsCollection.Name)]
    public class SearchArtistByNameFunctionTests
    {
        private readonly TestsInitializer _testsInitializer;
        private Mock<IMediator> _mediator;
        private SearchArtistByNameFunction _function;
        private HttpRequest _httpRequest;
        private IActionResult _result;

        public SearchArtistByNameFunctionTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;
            _mediator = new Mock<IMediator>();

            _function = new SearchArtistByNameFunction(_mediator.Object, new SearchArtistByNameResponseDtoFormatter());
        }

        [Fact]
        public Task ErrorOccursWhenSearchingForArtist()
        {
            this.Given(x => GivenErrorOccursWhenSearchingForArtist())
                .When(x => WhenFunctionTriggers())
                .Then(x => ThenMustReturnErrorResponse(HttpStatusCode.InternalServerError))
                .And(x => ThenErrorResponseMustContainErrorMessages())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task MultipleArtistsAreFoundFromSearch()
        {
            this.Given(x => GivenMultipleArtistsAreFoundFromSearch())
                .When(x => WhenFunctionTriggers())
                .Then(x => ThenMustReturnSuccessfulResponse())
                .And(x => ThenResponseMustContainMultipleArtistData())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task ArtistWasFoundButNoRecords()
        {
            this.Given(x => GivenArtistWasFound())
                .And(x => GivenNoRecordsWereFoundForArtist())
                .When(x => WhenFunctionTriggers())
                .Then(x => ThenMustReturnSuccessfulResponse())
                .And(x => ThenMustContainArtistData())
                .And(x => ThenMustContainEmptyArtistRecords())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Fact]
        public Task ArtistWasFoundWithMultipleRecords()
        {
            this.Given(x => GivenArtistWasFound())
                .And(x => GivenRecordsWereFoundForArtist())
                .When(x => WhenFunctionTriggers())
                .Then(x => ThenMustReturnSuccessfulResponse())
                .And(x => ThenMustContainArtistData())
                .And(x => ThenMustContainArtistRecords())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenMustContainArtistRecords()
        {
            var objectResult = (ObjectResult)(_result);
            objectResult.Should().NotBeNull();

            var response = (SearchArtistByNameResponse)(objectResult.Value);
            response.Should().NotBeNull();

            response.Records.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }

        private Task GivenRecordsWereFoundForArtist()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchRecordsForArtistByIdRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchRecordsForArtistByIdResponseDto>.Success(new SearchRecordsForArtistByIdResponseDto
                {
                    Records = _testsInitializer.Fixture.CreateMany<RecordDto>().ToList()
                }));

            return Task.CompletedTask;
        }

        private Task ThenMustContainEmptyArtistRecords()
        {
            var objectResult = (ObjectResult)(_result);
            objectResult.Should().NotBeNull();

            var response = (SearchArtistByNameResponse)(objectResult.Value);
            response.Should().NotBeNull();

            response.Records.Should().NotBeNull();
            response.Records.Should().BeEmpty();

            return Task.CompletedTask;
        }

        private Task ThenMustContainArtistData()
        {
            var objectResult = (ObjectResult) (_result);
            objectResult.Should().NotBeNull();

            var response = (SearchArtistByNameResponse) (objectResult.Value);
            response.Should().NotBeNull();
            response.ArtistId.Should().NotBeNullOrEmpty();
            response.ArtistName.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }

        private Task GivenNoRecordsWereFoundForArtist()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchRecordsForArtistByIdRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchRecordsForArtistByIdResponseDto>.Success(new SearchRecordsForArtistByIdResponseDto
                {
                    Records = new List<RecordDto>()
                }));

            return Task.CompletedTask;
        }

        private Task GivenArtistWasFound()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchArtistByNameRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchArtistByNameResponseDto>.Success(new SearchArtistByNameResponseDto
                {
                    Artists = new List<ArtistDto>(new []{_testsInitializer.Fixture.Create<ArtistDto>()})
                }));

            _httpRequest = _testsInitializer.GetMockedRequest(new Dictionary<string, StringValues>
            {
                {"correlationId", "somecorrelationid"}
            }, new Dictionary<string, StringValues>
            {
                {"name", "cheranga"}
            });

            return Task.CompletedTask;
        }

        private Task ThenResponseMustContainMultipleArtistData()
        {
            var objectResult = (ObjectResult)(_result);
            var response = (SearchArtistByNameResponseDto)(objectResult.Value);
            response.Should().NotBeNull();
            response.Artists.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccessfulResponse()
        {
            var objectResult = (ObjectResult)(_result);
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)(HttpStatusCode.OK));

            return Task.CompletedTask;
        }

        private Task GivenMultipleArtistsAreFoundFromSearch()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchArtistByNameRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchArtistByNameResponseDto>.Success(new SearchArtistByNameResponseDto
                {
                    Artists = _testsInitializer.Fixture.CreateMany<ArtistDto>().ToList()
                }));

            _httpRequest = _testsInitializer.GetMockedRequest(new Dictionary<string, StringValues>
            {
                {"correlationId", "somecorrelationid"}
            });

            return Task.CompletedTask;
        }

        private Task ThenErrorResponseMustContainErrorMessages()
        {
            var objectResponse = (ObjectResult)_result;
            var errorResponse = (ErrorResponse)objectResponse.Value;
            errorResponse.Should().NotBeNull();
            errorResponse.Errors.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnErrorResponse(HttpStatusCode expectedHttpStatusCode)
        {
            var objectResult = (ObjectResult)(_result);
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)expectedHttpStatusCode);

            return Task.CompletedTask;
        }

        private async Task WhenFunctionTriggers()
        {
            _result = await _function.SearchAsync(_httpRequest);
        }

        private Task GivenErrorOccursWhenSearchingForArtist()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchArtistByNameRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchArtistByNameResponseDto>.Failure("errorcode", "errormessage"));

            _httpRequest = _testsInitializer.GetMockedRequest(new Dictionary<string, StringValues>
            {
                {"correlationId", "somecorrelationid"}
            });

            return Task.CompletedTask;
        }
    }
}
