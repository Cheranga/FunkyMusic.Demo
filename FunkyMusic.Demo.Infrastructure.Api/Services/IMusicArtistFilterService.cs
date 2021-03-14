using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;

namespace FunkyMusic.Demo.Infrastructure.Api.Services
{
    public interface IMusicArtistFilterService
    {
        Result<MusicArtistSearchResponseDto> FilterByScore(Result<MusicArtistSearchResponseDto> result);
    }
}