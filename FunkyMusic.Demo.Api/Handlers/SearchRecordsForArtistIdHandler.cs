using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Api.Dto.Assets;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Api.Handlers
{
    public class SearchRecordsForArtistIdHandler : IRequestHandler<SearchRecordsForArtistByIdRequestDto, Result<SearchRecordsForArtistByIdResponseDto>>
    {
        private readonly IMediator _mediator;

        public SearchRecordsForArtistIdHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<SearchRecordsForArtistByIdResponseDto>> Handle(SearchRecordsForArtistByIdRequestDto request, CancellationToken cancellationToken)
        {
            var getRecordsForArtistIdRequest = new GetRecordsForArtistByIdRequest
            {
                CorrelationId = request.CorrelationId,
                ArtistId = request.ArtistId
            };

            var operation = await _mediator.Send(getRecordsForArtistIdRequest, cancellationToken);
            if (!operation.Status)
            {
                return Result<SearchRecordsForArtistByIdResponseDto>.Failure(operation.ErrorCode, operation.Validation);
            }

            var recordDtos = operation.Data.Records.Select(x => new RecordDto
            {
                Title = x.Name,
                ReleaseDate = x.ReleaseDate
            }).ToList();

            var response = new SearchRecordsForArtistByIdResponseDto
            {
                Records = recordDtos
            };

            return Result<SearchRecordsForArtistByIdResponseDto>.Success(response);
        }
    }
}