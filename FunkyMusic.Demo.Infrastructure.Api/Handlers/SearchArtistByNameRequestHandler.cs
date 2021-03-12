using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Models;
using MediatR;

namespace FunkyMusic.Demo.Infrastructure.Api.Handlers
{
    public class SearchArtistByNameRequestHandler : IRequestHandler<SearchArtistByNameRequest, Result<List<Artist>>>
    {
        public async Task<Result<List<Artist>>> Handle(SearchArtistByNameRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            var artists = new List<Artist>();
            return Result<List<Artist>>.Success(artists);
        }
    }
}