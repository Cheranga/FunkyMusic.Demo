using System;
using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using MediatR;

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
                return Result<GetArtistByNameResponse>.Failure(operation.Validation);
            }

            var response = new GetArtistByNameResponse
            {

            };

            return Result<GetArtistByNameResponse>.Success(response);
        }
    }
}