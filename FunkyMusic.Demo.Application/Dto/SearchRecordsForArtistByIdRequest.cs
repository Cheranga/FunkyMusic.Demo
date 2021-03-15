using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Application.Dto
{
    [ExcludeFromCodeCoverage]
    public class SearchRecordsForArtistByIdRequest : IRequest<Result<SearchRecordsForArtistByIdResponse>>, IValidatable
    {
        public string ArtistId { get; set; }
        public string CorrelationId { get; set; }
    }
}