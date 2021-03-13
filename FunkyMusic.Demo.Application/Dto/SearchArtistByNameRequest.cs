using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Models;
using MediatR;

namespace FunkyMusic.Demo.Application.Dto
{
    [ExcludeFromCodeCoverage]
    public class SearchArtistByNameRequest : IRequest<Result<List<Artist>>>, IValidatable
    {
        public string Name { get; set; }
        public string CorrelationId { get; set; }
    }
}