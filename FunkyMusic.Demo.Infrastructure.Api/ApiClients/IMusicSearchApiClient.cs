using System.Threading.Tasks;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;

namespace FunkyMusic.Demo.Infrastructure.Api.ApiClients
{
    public interface IMusicSearchApiClient
    {
        Task<Result<MusicArtistSearchResponseDto>> SearchArtistsByNameAsync(string artistName);
        Task<Result<MusicArtistRecordSearchResponseDto>> GetRecordsForArtistByIdAsync(string artistId);
    }
}