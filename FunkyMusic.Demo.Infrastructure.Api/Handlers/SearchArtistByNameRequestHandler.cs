using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using FunkyMusic.Demo.Domain.Models;
using FunkyMusic.Demo.Infrastructure.Api.ApiClients;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Assets;
using FunkyMusic.Demo.Infrastructure.Api.Services;
using MediatR;

namespace FunkyMusic.Demo.Infrastructure.Api.Handlers
{
    internal class SearchArtistByNameRequestHandler : IRequestHandler<SearchArtistByNameRequest, Result<SearchArtistByNameResponse>>
    {
        private readonly IMusicSearchApiClient _musicSearchApiClient;
        private readonly IMusicArtistFilterService _musicArtistFilter;

        public SearchArtistByNameRequestHandler(IMusicSearchApiClient musicSearchApiClient, IMusicArtistFilterService musicArtistFilter)
        {
            _musicSearchApiClient = musicSearchApiClient;
            _musicArtistFilter = musicArtistFilter;
        }

        public async Task<Result<SearchArtistByNameResponse>> Handle(SearchArtistByNameRequest request, CancellationToken cancellationToken)
        {
            var operation = await _musicSearchApiClient.SearchArtistsByNameAsync(request.Name);
            if (!operation.Status)
            {
                return Result<SearchArtistByNameResponse>.Failure(operation.ErrorCode, operation.Validation);
            }

            operation = _musicArtistFilter.FilterByScore(operation);

            var artistDtos = operation.Data?.Artists ?? new List<MusicArtistDto>();
            if (!artistDtos.Any())
            {
                return Result<SearchArtistByNameResponse>.Failure(ErrorCodes.ArtistNotFound, "Artist not found.");
            }

            var artists = artistDtos.Select(x => new Artist
            {
                Name = x.Name,
                Id = x.Id,
                Description = x.Disambiguation
            }).ToList();

            return Result<SearchArtistByNameResponse>.Success(new SearchArtistByNameResponse
            {
                Artists = artists
            });
        }
    }
}