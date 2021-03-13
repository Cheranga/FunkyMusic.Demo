using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Application.Responses;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Application.Requests
{
    [ExcludeFromCodeCoverage]
    public class GetArtistByNameRequest : IRequest<Result<GetArtistByNameResponse>>, IValidatable
    {
        public string Name { get; set; }
        public string CorrelationId { get; set; }
    }
}