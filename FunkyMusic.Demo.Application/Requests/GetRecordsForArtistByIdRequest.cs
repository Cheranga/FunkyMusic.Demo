using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Application.Requests
{
    public class GetRecordsForArtistByIdRequest : IRequest<Result<GetRecordsForArtistByIdResponse>>, IValidatable
    {
        public string ArtistId { get; set; }
        public string CorrelationId { get; set; }
    }
}