using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using MediatR;
using Record = FunkyMusic.Demo.Domain.Models.Record;

namespace FunkyMusic.Demo.Application.Handlers
{
    internal class GetRecordsForArtistByIdRequestHandler : IRequestHandler<GetRecordsForArtistByIdRequest, Result<GetRecordsForArtistByIdResponse>>
    {
        private readonly IMediator _mediator;

        public GetRecordsForArtistByIdRequestHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<GetRecordsForArtistByIdResponse>> Handle(GetRecordsForArtistByIdRequest request, CancellationToken cancellationToken)
        {
            var searchRecordsForArtistRequest = new SearchRecordsForArtistByIdRequest
            {
                CorrelationId = request.CorrelationId,
                ArtistId = request.ArtistId
            };

            var operation = await _mediator.Send(searchRecordsForArtistRequest, cancellationToken);
            if (!operation.Status)
            {
                return Result<GetRecordsForArtistByIdResponse>.Failure(operation.ErrorCode, operation.Validation);
            }

            var records = operation.Data?.ToList() ?? new List<Record>();
            if (!records.Any())
            {
                return Result<GetRecordsForArtistByIdResponse>.Failure(ErrorCodes.ArtistRecordsNotFound, "No records found for the artist.");
            }

            var recordModels = records.Select(x => new Responses.Record
            {
                Id = x.Id,
                Name = x.Title
            }).ToList();

            return Result<GetRecordsForArtistByIdResponse>.Success(new GetRecordsForArtistByIdResponse
            {
                Records = recordModels
            });
        }
    }
}