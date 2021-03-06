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
    public class SearchArtistByNameResponseDtoFormatter : IResponseFormatter<SearchArtistByNameResponseDto>
    {
        private const string ErrorOccuredWhenSearchingForArtist = "Error occured when searching for artist.";
        private const string ArtistNotFound = "Artist not found.";

        public IActionResult GetActionResult(Result<SearchArtistByNameResponseDto> result)
        {
            if (result == null)
            {
                return GetErrorResponse(Result<SearchArtistByNameResponseDto>.Failure(ErrorCodes.ArtistSearchError, ErrorOccuredWhenSearchingForArtist));
            }

            if (!result.Status)
            {
                return GetErrorResponse(result);
            }

            return new OkObjectResult(result.Data);
        }

        private IActionResult GetErrorResponse(Result<SearchArtistByNameResponseDto> result)
        {
            var errorCode = result.ErrorCode;

            ErrorResponse errorResponse;
            HttpStatusCode statusCode;

            switch (errorCode)
            {
                case ErrorCodes.ValidationError:
                    errorResponse = GetArtistValidationErrorResponse(result);
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case ErrorCodes.ArtistNotFound:
                    errorResponse = GetArtistNotFoundResponse();
                    statusCode = HttpStatusCode.NotFound;
                    break;

                default:
                    errorResponse = GetArtistSearchErrorResponse(ErrorCodes.ArtistSearchError);
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            return new ObjectResult(errorResponse)
            {
                StatusCode = (int)statusCode
            };
        }

        private ErrorResponse GetArtistNotFoundResponse()
        {
            var errorResponse = new ErrorResponse
            {
                ErrorCode = ErrorCodes.ArtistNotFound,
                Errors = new List<ErrorMessage>
                {
                    new ErrorMessage {Field = "Artist", Message = ArtistNotFound}
                }
            };

            return errorResponse;
        }

        private ErrorResponse GetArtistValidationErrorResponse(Result<SearchArtistByNameResponseDto> result)
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

        private ErrorResponse GetArtistSearchErrorResponse(string errorCode)
        {
            var errorResponse = new ErrorResponse
            {
                ErrorCode = errorCode,
                Errors = new List<ErrorMessage>
                {
                    new ErrorMessage {Field = "", Message = ErrorOccuredWhenSearchingForArtist}
                }
            };

            return errorResponse;
        }
    }
}