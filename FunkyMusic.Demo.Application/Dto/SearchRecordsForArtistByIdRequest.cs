using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Models;
using MediatR;

namespace FunkyMusic.Demo.Application.Dto
{
    [ExcludeFromCodeCoverage]
    public class SearchRecordsForArtistByIdRequest : IRequest<Result<List<Record>>>, IValidatable
    {
        public string ArtistId { get; set; }
        public string CorrelationId { get; set; }
    }
}