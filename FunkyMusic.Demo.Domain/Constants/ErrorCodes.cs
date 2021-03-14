namespace FunkyMusic.Demo.Domain.Constants
{
    public class ErrorCodes
    {
        public const string ApplicationError = "APPLICATION_ERROR_500";
        public const string ValidationError = "VALIDATION_400";
        public const string ArtistSearchError = "ARTIST_SEARCH_ERROR_500";
        public const string ArtistRecordsSearchError = "ARTIST_RECORDS_SEARCH_ERROR_500";
        public const string ArtistNotFound = "ARTIST_404";
        public const string ArtistRecordsNotFound = "ARTIST_RECORDS_404";
        public const string MusicSearchError = "MUSIC_SEARCH_API_ERROR";
    }
}