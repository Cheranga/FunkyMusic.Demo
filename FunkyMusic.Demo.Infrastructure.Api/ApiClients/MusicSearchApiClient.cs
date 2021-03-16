using System;
using System.Net.Http;
using System.Threading.Tasks;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using FunkyMusic.Demo.Infrastructure.Api.Configs;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunkyMusic.Demo.Infrastructure.Api.ApiClients
{
    public class MusicSearchApiClient : IMusicSearchApiClient
    {
        private const string ErrorWhenSearchingForArtist = "Error occured when searching for artist.";
        private const string ErrorWhenSearchingRecordingsForArtist = "Error occured when searching recordings for artist.";
        private const string AcceptHeaderValue = "application/json";
        private readonly HttpClient _httpClient;
        private readonly ILogger<MusicSearchApiClient> _logger;
        private readonly MusicSearchConfig _musicSearchConfig;

        public MusicSearchApiClient(HttpClient httpClient, MusicSearchConfig musicSearchConfig, ILogger<MusicSearchApiClient> logger)
        {
            _httpClient = httpClient;
            _musicSearchConfig = musicSearchConfig;
            _logger = logger;
        }


        public async Task<Result<MusicArtistSearchResponseDto>> SearchArtistsByNameAsync(string artistName)
        {
            try
            {
                var uri = new Uri($"{_musicSearchConfig.Url}/artist?query={artistName}");
                var httpRequest = GetHttpRequest(uri, HttpMethod.Get);

                var httpResponse = await _httpClient.SendAsync(httpRequest);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error occured when calling music search API {apiReason}", httpResponse.ReasonPhrase);
                    return Result<MusicArtistSearchResponseDto>.Failure(ErrorCodes.MusicSearchError, ErrorWhenSearchingForArtist);
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var artistSearchDto = JsonConvert.DeserializeObject<MusicArtistSearchResponseDto>(responseContent);

                if (artistSearchDto == null)
                {
                    return Result<MusicArtistSearchResponseDto>.Failure(ErrorCodes.MusicSearchError, ErrorWhenSearchingForArtist);
                }

                return Result<MusicArtistSearchResponseDto>.Success(artistSearchDto);
            }
            catch (Exception exception)
            {
                _logger.LogError("The config data {url}, {applicationId}, {confidence}", _musicSearchConfig.Url, _musicSearchConfig.ApplicationId, _musicSearchConfig.MinConfidenceForArtistFilter);
                _logger.LogError(exception, ErrorWhenSearchingForArtist);
            }

            return Result<MusicArtistSearchResponseDto>.Failure(ErrorCodes.MusicSearchError, ErrorWhenSearchingForArtist);
        }


        public async Task<Result<MusicArtistRecordSearchResponseDto>> GetRecordsForArtistByIdAsync(string artistId)
        {
            try
            {
                var uri = new Uri($"{_musicSearchConfig.Url}/artist/{artistId}?inc=recordings");
                var httpRequest = GetHttpRequest(uri, HttpMethod.Get);

                var httpResponse = await _httpClient.SendAsync(httpRequest);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error occured when calling music search API {apiReason}", httpResponse.ReasonPhrase);
                    return Result<MusicArtistRecordSearchResponseDto>.Failure(ErrorCodes.MusicSearchError, ErrorWhenSearchingRecordingsForArtist);
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var artistRecordingsSearchDto = JsonConvert.DeserializeObject<MusicArtistRecordSearchResponseDto>(responseContent);

                if (artistRecordingsSearchDto == null)
                {
                    return Result<MusicArtistRecordSearchResponseDto>.Failure(ErrorCodes.MusicSearchError, ErrorWhenSearchingRecordingsForArtist);
                }

                return Result<MusicArtistRecordSearchResponseDto>.Success(artistRecordingsSearchDto);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, ErrorWhenSearchingForArtist);
            }

            return Result<MusicArtistRecordSearchResponseDto>.Failure(ErrorCodes.MusicSearchError, ErrorWhenSearchingRecordingsForArtist);
        }

        private HttpRequestMessage GetHttpRequest(Uri uri, HttpMethod httpMethod)
        {
            var httpRequest = new HttpRequestMessage(httpMethod, uri)
            {
                Headers =
                {
                    {"User-Agent", _musicSearchConfig.ApplicationId},
                    {"Accept", AcceptHeaderValue}
                }
            };

            return httpRequest;
        }
    }
}