using System.Collections.Generic;
using System.Linq;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Infrastructure.Api.Configs;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Assets;
using FunkyMusic.Demo.Infrastructure.Api.Dto.Responses;

namespace FunkyMusic.Demo.Infrastructure.Api.Services
{
    internal class MusicArtistFilterService : IMusicArtistFilterService
    {
        readonly int _minScore = 100;

        public MusicArtistFilterService(MusicSearchConfig musicSearchConfig)
        {
            _minScore = (musicSearchConfig == null || musicSearchConfig.MinScoreForArtistFilter <= 0) ? 100 : musicSearchConfig.MinScoreForArtistFilter;
        }

        public Result<MusicArtistSearchResponseDto> FilterByScore(Result<MusicArtistSearchResponseDto> result)
        {
            if (result == null || !result.Status)
            {
                return result;
            }

            var artists = result.Data?.Artists ?? new List<MusicArtistDto>();
            if (!artists.Any())
            {
                return result;
            }

            var filteredArtists = artists.GroupBy(x => x.Score).Where(x => x.Key >= _minScore).SelectMany(x => x.ToList()).ToList();

            return Result<MusicArtistSearchResponseDto>.Success(new MusicArtistSearchResponseDto
            {
                Artists = filteredArtists
            });
        }
        
    }
}