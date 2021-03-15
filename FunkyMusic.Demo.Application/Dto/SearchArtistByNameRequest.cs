using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Application.Dto
{
    [ExcludeFromCodeCoverage]
    public class SearchArtistByNameRequest : IRequest<Result<SearchArtistByNameResponse>>, IValidatable
    {
        public string Name { get; set; }
        public string CorrelationId { get; set; }
    }
}