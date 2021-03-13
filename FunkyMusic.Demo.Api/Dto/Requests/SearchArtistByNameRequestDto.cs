using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Api.Dto.Requests
{
    [ExcludeFromCodeCoverage]
    public class SearchArtistByNameRequestDto : IRequest<Result<SearchArtistByNameResponseDto>>, IValidatable
    {
        public string ArtistName { get; set; }
        public string CorrelationId { get; set; }
    }
}