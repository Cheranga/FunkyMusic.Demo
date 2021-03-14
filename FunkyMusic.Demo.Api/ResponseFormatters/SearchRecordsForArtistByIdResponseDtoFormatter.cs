using System.Collections.Generic;
using System.Linq;
using System.Net;
using FunkyMusic.Demo.Api.Dto.Assets;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace FunkyMusic.Demo.Api.ResponseFormatters
{
    public class SearchRecordsForArtistByIdResponseDtoFormatter : IResponseFormatter<SearchRecordsForArtistByIdResponseDto>
    {
        private const string NoRecordsFoundForArtist = "There are no records found for the artist.";
        private const string ErrorOccuredWhenSearchingForRecords = "Error occured when searching records for artist.";

        public IActionResult GetActionResult(Result<SearchRecordsForArtistByIdResponseDto> result)
        {
            if (result == null)
            {
                return GetErrorResponse(Result<SearchRecordsForArtistByIdResponseDto>.Failure(ErrorCodes.ArtistRecordsSearchError, ErrorOccuredWhenSearchingForRecords));
            }

            if (!result.Status)
            {
                return GetErrorResponse(result);
            }

            return new OkObjectResult(result.Data);
        }

        private IActionResult GetErrorResponse(Result<SearchRecordsForArtistByIdResponseDto> result)
        {
            var errorCode = result.ErrorCode;

            ErrorResponse errorResponse;
            HttpStatusCode statusCode;

            switch (errorCode)
            {
                case ErrorCodes.ValidationError:
                    errorResponse = GetArtistRecordSearchValidationErrorResponse(result);
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case ErrorCodes.ArtistRecordsNotFound:
                    errorResponse = GetArtistRecordsNotFoundResponse();
                    statusCode = HttpStatusCode.NotFound;
                    break;

                default:
                    errorResponse = GetArtistRecordSearchErrorResponse(ErrorCodes.ArtistRecordsSearchError);
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            return new ObjectResult(errorResponse)
            {
                StatusCode = (int)statusCode
            };
        }

        private ErrorResponse GetArtistRecordsNotFoundResponse()
        {
            var errorResponse = new ErrorResponse
            {
                ErrorCode = ErrorCodes.ArtistRecordsNotFound,
                Errors = new List<ErrorMessage>
                {
                    new ErrorMessage {Field = "ArtistRecords", Message = NoRecordsFoundForArtist}
                }
            };

            return errorResponse;
        }

        private ErrorResponse GetArtistRecordSearchValidationErrorResponse(Result<SearchRecordsForArtistByIdResponseDto> result)
        {
            var errorResponse = new ErrorResponse
            {
                ErrorCode = ErrorCodes.ValidationError,
                Errors = result.Validation.Errors.Select(x => new ErrorMessage
                {
                    Field = x.PropertyName,
                    Message = x.ErrorMessage
                }).ToList()
            };

            return errorResponse;
        }

        private ErrorResponse GetArtistRecordSearchErrorResponse(string errorCode)
        {
            var errorResponse = new ErrorResponse
            {
                ErrorCode = errorCode,
                Errors = new List<ErrorMessage>
                {
                    new ErrorMessage {Field = "", Message = ErrorOccuredWhenSearchingForRecords}
                }
            };

            return errorResponse;
        }
    }
}