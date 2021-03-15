using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Application.Handlers;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using MediatR;
using Moq;
using TestStack.BDDfy;
using Xunit;
using Record = FunkyMusic.Demo.Domain.Models.Record;

namespace FunkyMusic.Demo.Application.Tests.Handlers
{
    [Collection(MusicDemoApplicationTestsCollection.Name)]
    public class GetRecordsForArtistByIdRequestHandlerTests
    {
        public GetRecordsForArtistByIdRequestHandlerTests(TestsInitializer testsInitializer)
        {
            _fixture = testsInitializer.Fixture;
            _mediator = new Mock<IMediator>();
            _handler = new GetRecordsForArtistByIdRequestHandler(_mediator.Object);
            _request = testsInitializer.Fixture.Create<GetRecordsForArtistByIdRequest>();
        }

        private readonly Mock<IMediator> _mediator;
        private GetRecordsForArtistByIdRequestHandler _handler;
        private GetRecordsForArtistByIdRequest _request;
        private Result<GetRecordsForArtistByIdResponse> _result;
        private static Fixture _fixture;

        private Task GivenThirdPartyMusicServiceReturnsError()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchRecordsForArtistByIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchRecordsForArtistByIdResponse>.Failure("errorcode", "errormessage"));

            return Task.CompletedTask;
        }

        [Fact]
        public Task MusicSearchFails()
        {
            this.Given(x => GivenThirdPartyMusicServiceReturnsError())
                .When(x => WhenApplicationRequestHandlerExcutes())
                .Then(x => ThenMustReturnFailure())
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [MemberData(nameof(GetFailedRecords))]
        public Task ArtistNotFound(List<Domain.Models.Record> records)
        {
            this.Given(x => GivenNoRecordsCanBeFoundForArtist(records))
                .When(x => WhenApplicationRequestHandlerExcutes())
                .Then(x => ThenMustReturnFailure())
                .And(x => ThenMustReturnErrorCode(ErrorCodes.ArtistRecordsNotFound))
                .BDDfy();

            return Task.CompletedTask;
        }

        [Theory]
        [MemberData(nameof(GetFoundRecordsForArtist))]
        public Task ArtistFound(List<Record> records)
        {
            this.Given(x => GivenThereAreRecordsForArtist(records))
                .When(x => WhenApplicationRequestHandlerExcutes())
                .Then(x => ThenMustReturnSuccess())
                .And(x => ThenResponseMustContainRecords())
                .BDDfy();

            return Task.CompletedTask;
        }

        private Task ThenResponseMustContainRecords()
        {
            _result.Data.Should().NotBeNull();
            _result.Data.Records.Should().NotBeNullOrEmpty();

            return Task.CompletedTask;
        }

        private Task ThenMustReturnSuccess()
        {
            _result.Status.Should().BeTrue();
            return Task.CompletedTask;
        }

        private Task GivenThereAreRecordsForArtist(List<Record> records)
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchRecordsForArtistByIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchRecordsForArtistByIdResponse>.Success(new SearchRecordsForArtistByIdResponse
                {
                    Records = records
                }));

            return Task.CompletedTask;
        }

        private Task ThenMustReturnErrorCode(string expectedErrorCode)
        {
            _result.ErrorCode.Should().BeEquivalentTo(expectedErrorCode);
            return Task.CompletedTask;
        }

        private Task GivenNoRecordsCanBeFoundForArtist(List<Record> records)
        {
            _mediator.Setup(x => x.Send(It.IsAny<SearchRecordsForArtistByIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<SearchRecordsForArtistByIdResponse>.Success(new SearchRecordsForArtistByIdResponse
                {
                    Records = records
                }));

            return Task.CompletedTask;

        }

        private Task ThenMustReturnFailure()
        {
            _result.Should().NotBeNull();
            _result.Status.Should().BeFalse();

            return Task.CompletedTask;
        }

        private async Task WhenApplicationRequestHandlerExcutes()
        {
            _result = await _handler.Handle(_request, CancellationToken.None);
        }

        public static IEnumerable<object[]> GetFailedRecords()
        {
            var artists = new List<object[]>
            {
                new object[] {null},
                new object[] {new List<Domain.Models.Record>()}
            };

            return artists;
        }

        public static IEnumerable<object[]> GetFoundRecordsForArtist()
        {
            var artists = new List<object[]>
            {
                new object[] {new List<Record>{ _fixture.Create<Record>() } },
                new object[] {_fixture.CreateMany<Record>().ToList()}
            };

            return artists;
        }
    }
}