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
using MediatR;

namespace FunkyMusic.Demo.Infrastructure.Api.Handlers
{
    internal class SearchRecordsForArtistByIdRequestHandler : IRequestHandler<SearchRecordsForArtistByIdRequest, Result<List<Record>>>
    {
        private readonly IMusicSearchApiClient _musicSearchApiClient;

        public SearchRecordsForArtistByIdRequestHandler(IMusicSearchApiClient musicSearchApiClient)
        {
            _musicSearchApiClient = musicSearchApiClient;
        }

        public async Task<Result<List<Record>>> Handle(SearchRecordsForArtistByIdRequest request, CancellationToken cancellationToken)
        {
            var operation = await _musicSearchApiClient.GetRecordsForArtistByIdAsync(request.ArtistId);

            if (!operation.Status)
            {
                return Result<List<Record>>.Failure(operation.ErrorCode, operation.Validation);
            }

            var recordDtos = operation.Data?.Recordings ?? new List<MusicRecordDto>();
            if (!recordDtos.Any())
            {
                return Result<List<Record>>.Failure(ErrorCodes.ArtistRecordsNotFound, "Records were not found for artist.");
            }

            var records = recordDtos.Select(x => new Record
            {
                Id = x.Id,
                Length = x.Length.HasValue? x.Length.Value : 0,
                Title = x.Title
            }).ToList();

            return Result<List<Record>>.Success(records);
        }
    }
}