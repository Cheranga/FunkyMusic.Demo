using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Application.Requests;
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
                return Result<SearchArtistByNameResponseDto>.Failure(operation.Validation);
            }

            // TODO: Some elegant mapping.
            var response = new SearchArtistByNameResponseDto();
            return Result<SearchArtistByNameResponseDto>.Success(response);
        }
    }
}