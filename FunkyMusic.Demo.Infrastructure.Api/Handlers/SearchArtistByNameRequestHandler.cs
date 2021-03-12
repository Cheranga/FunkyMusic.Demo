using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using FunkyMusic.Demo.Domain.Models;
using MediatR;

namespace FunkyMusic.Demo.Infrastructure.Api.Handlers
{
    internal class SearchArtistByNameRequestHandler : IRequestHandler<SearchArtistByNameRequest, Result<List<Artist>>>
    {
        public Task<Result<List<Artist>>> Handle(SearchArtistByNameRequest request, CancellationToken cancellationToken)
        {
            var artists = Enumerable.Range(1, 10).Select(x => new Artist
            {
                Id = x.ToString(),
                Name = $"Artist{x}",
                Country = "AU",
                Gender = x % 2 == 0 ? Gender.Male : Gender.Female,
                Type = x % 2 == 0 ? ArtistType.Person : ArtistType.Group
            }).ToList();

            return Task.FromResult(Result<List<Artist>>.Success(artists));
        }
    }
}