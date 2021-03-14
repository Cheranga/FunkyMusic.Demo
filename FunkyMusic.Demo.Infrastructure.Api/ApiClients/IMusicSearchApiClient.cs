using System.Collections.Generic;
using System.Threading.Tasks;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Assets;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;

namespace FunkyMusic.Demo.Infrastructure.Api.ApiClients
{
    public interface IMusicSearchApiClient
    {
        Task<Result<MusicArtistSearchResponseDto>> SearchArtistsByNameAsync(string artistName);
        //Task<Result<MusicArtistDto>> GetArtistByIdAsync(string artistId);
        Task<Result<MusicArtistRecordSearchResponseDto>> GetRecordsForArtistByIdAsync(string artistId);
    }
}