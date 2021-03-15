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
    internal class SearchRecordsForArtistByIdRequestHandler : IRequestHandler<SearchRecordsForArtistByIdRequest, Result<SearchRecordsForArtistByIdResponse>>
    {
        private readonly IMusicSearchApiClient _musicSearchApiClient;

        public SearchRecordsForArtistByIdRequestHandler(IMusicSearchApiClient musicSearchApiClient)
        {
            _musicSearchApiClient = musicSearchApiClient;
        }

        public async Task<Result<SearchRecordsForArtistByIdResponse>> Handle(SearchRecordsForArtistByIdRequest request, CancellationToken cancellationToken)
        {
            var operation = await _musicSearchApiClient.GetRecordsForArtistByIdAsync(request.ArtistId);

            if (!operation.Status)
            {
                return Result<SearchRecordsForArtistByIdResponse>.Failure(operation.ErrorCode, operation.Validation);
            }

            var recordDtos = operation.Data?.Recordings ?? new List<MusicRecordDto>();
            if (!recordDtos.Any())
            {
                return Result<SearchRecordsForArtistByIdResponse>.Failure(ErrorCodes.ArtistRecordsNotFound, "Records were not found for artist.");
            }

            var records = recordDtos.Select(x => new Record
            {
                Id = x.Id,
                Length = x.Length ?? 0,
                Title = x.Title
            }).ToList();

            return Result<SearchRecordsForArtistByIdResponse>.Success(new SearchRecordsForArtistByIdResponse
            {
                Records = records
            });
        }
    }
}