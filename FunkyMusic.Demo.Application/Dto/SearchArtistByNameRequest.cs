using System.Collections.Generic;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Models;
using MediatR;

namespace FunkyMusic.Demo.Application.Dto
{
    public class SearchArtistByNameRequest : IRequest<Result<List<Artist>>>, IValidatable
    {
        public string Name { get; set; }
        public string CorrelationId { get; set; }
    }
}