using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Api.Handlers;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using MediatR;
using Moq;
using TestStack.BDDfy;
using Xunit;
using Record = FunkyMusic.Demo.Application.Responses.Record;

namespace FunkyMusic.Demo.Api.Tests.Handlers
{
    [Collection(MusicDemoApiTestsCollection.Name)]
    public class SearchRecordsForArtistIdHandlerTests
    {
        private static TestsInitializer _testsInitializer;
        private Mock<IMediator> _mediator;
        private SearchRecordsForArtistIdHandler _handler;
        private SearchRecordsForArtistByIdRequestDto _request;
        private Result<SearchRecordsForArtistByIdResponseDto> _result;

        public SearchRecordsForArtistIdHandlerTests(TestsInitializer testsInitializer)
        {
            _testsInitializer = testsInitializer;

            _request = _testsInitializer.Fixture.Create<SearchRecordsForArtistByIdRequestDto>();
            _mediator = new Mock<IMediator>();
            _handler = new SearchRecordsForArtistIdHandler(_mediator.Object);
        }

        [Fact]
        public Task ErrorOccuredWhenSearchingForRecords()
        {
            this.Given(x => GivenErrorOccuredWhenSearchingForRecords())
                .When(x => WhenHandlerExecutes())
                .Then(x => ThenMustReturnFailure())
                .BDDfy();

            return Task.CompletedTask;
        }
        
        [Theory]
        [MemberData(nameof(GetNotFoundRecords))]
        public Task ArtistRecordsAreNotFound(List<Record> records)
        {
            this.Given(x => GivenMusicSearchReturnsRecords(records))
                .When(x => WhenTheHandlerExecutes())
                .Then(x => ThenMustReturnFailure())
                .And(x => ThenMustHaveErrorCode(ErrorCodes.ArtistRecordsNotFound))
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [MemberData(nameof(GetFoundRecords))]
        public Task ArtistRecordsAreFound(List<Record> records)
        {
            this.Given(x => GivenMusicSearchReturnsRecords(records))
                .When(x => WhenTheHandlerExecutes())
                .Then(x => ThenMustReturnSuccess())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccess()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeTrue();

            return Task.CompletedTask;
        }

        private Task ThenMustHaveErrorCode(string expectedErrorCode)
        {
            _result.ErrorCode.Should().BeEquivalentTo(expectedErrorCode);

            return Task.CompletedTask;
        }

        private async Task WhenTheHandlerExecutes()
        {
            _result = await _handler.Handle(_request, CancellationToken.None);
        }

        private Task GivenMusicSearchReturnsRecords(List<Record> records)
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetRecordsForArtistByIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetRecordsForArtistByIdResponse>.Success(new GetRecordsForArtistByIdResponse{Records = records}));

            return Task.CompletedTask;
        }

        public static IEnumerable<object[]> GetNotFoundRecords()
        {
            var artists = new List<object[]>
            {
                new object[] {null},
                new object[] {new List<Record>()}
            };

            return artists;
        }

        public static IEnumerable<object[]> GetFoundRecords()
        {
            var artists = new List<object[]>
            {
                new object[] {new List<Record>(new[] {_testsInitializer.Fixture.Create<Record>()})},
                new object[] {new List<Record>(_testsInitializer.Fixture.CreateMany<Record>())}
            };

            return artists;
        }

        private Task ThenMustReturnFailure()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeFalse();

            return Task.CompletedTask;
        }

        private async Task WhenHandlerExecutes()
        {
            _result = await _handler.Handle(_request, CancellationToken.None);
        }

        private Task GivenErrorOccuredWhenSearchingForRecords()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetRecordsForArtistByIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetRecordsForArtistByIdResponse>.Failure("errorcode", "errormessage"));

            return Task.CompletedTask;
        }
    }
}
