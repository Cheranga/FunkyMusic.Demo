using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Api.Dto.Assets;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Api.Handlers
{
    public class SearchArtistByNameHandler : IRequestHandler<SearchArtistByNameRequestDto, Result<SearchArtistByNameResponseDto>>
    {
        private readonly IMediator _mediator;

        public SearchArtistByNameHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<SearchArtistByNameResponseDto>> Handle(SearchArtistByNameRequestDto request, CancellationToken cancellationToken)
        {
            var getArtistByNameRequest = new GetArtistByNameRequest
            {
                CorrelationId = request.CorrelationId,
                Name = request.ArtistName
            };

            var operation = await _mediator.Send(getArtistByNameRequest, cancellationToken);
            if (!operation.Status)
            {
                return Result<SearchArtistByNameResponseDto>.Failure(operation.ErrorCode, operation.Validation);
            }

            var artistDtos = operation.Data.Artists.Select(x => new ArtistDto
            {
                ArtistId = x.ArtistId,
                ArtistName = x.ArtistName
            }).ToList();

            var response = new SearchArtistByNameResponseDto
            {
                Artists = artistDtos
            };

            return Result<SearchArtistByNameResponseDto>.Success(response);
        }
    }
}