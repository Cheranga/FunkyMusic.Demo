namespace FunkyMusic.Demo.Domain.Constants
{
    public class ErrorCodes
    {
        public const string ValidationError = "VALIDATION_400";
        public const string ArtistSearchExternalError = "ARTIST_SEARCH_EXTERNAL_ERROR_500";
        public const string ArtistSearchInternalError = "ARTIST_SEARCH_INTERNAL_ERROR_500";
        public const string ArtistRecordsSearchExternalError = "ARTIST_RECORDS_SEARCH_EXTERNAL_ERROR_500";
        public const string ArtistRecordsSearchInternalError = "ARTIST_RECORDS_SEARCH_INTERNAL_ERROR_500";
        public const string ArtistNotFound = "ARTIST_404";
        public const string ArtistRecordsNotFound = "ARTIST_RECORDS_404";
    }
}