using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Application.Requests
{
    public class GetArtistByNameRequest : IRequest<Result<GetArtistByNameResponse>>, IValidatable
    {
        public string Name { get; set; }
        public string CorrelationId { get; set; }
    }
}