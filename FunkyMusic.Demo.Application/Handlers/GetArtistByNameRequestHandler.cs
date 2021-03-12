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
using Artist = FunkyMusic.Demo.Domain.Models.Artist;

namespace FunkyMusic.Demo.Application.Handlers
{
    internal class GetArtistByNameRequestHandler : IRequestHandler<GetArtistByNameRequest, Result<GetArtistByNameResponse>>
    {
        private readonly IMediator _mediator;

        public GetArtistByNameRequestHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<GetArtistByNameResponse>> Handle(GetArtistByNameRequest request, CancellationToken cancellationToken)
        {
            var searchArtistByNameRequest = new SearchArtistByNameRequest
            {
                CorrelationId = request.CorrelationId,
                Name = request.Name
            };

            var operation = await _mediator.Send(searchArtistByNameRequest, cancellationToken);
            if (!operation.Status)
            {
                return Result<GetArtistByNameResponse>.Failure(operation.ErrorCode, operation.Validation);
            }

            var response = GetArtistResponse(operation.Data);

            return response;
        }

        private Result<GetArtistByNameResponse> GetArtistResponse(List<Artist> operationData)
        {
            var artists = operationData?.ToList() ?? new List<Artist>();

            if (!artists.Any())
            {
                return Result<GetArtistByNameResponse>.Failure(ErrorCodes.ArtistNotFound, "Artist not found");
            }

            var artistModels = artists.Select(x => new Responses.Artist
            {
                ArtistId = x.Id,
                ArtistName = x.Name
            }).ToList();

            return Result<GetArtistByNameResponse>.Success(new GetArtistByNameResponse
            {
                Artists = artistModels
            });
        }
    }
}