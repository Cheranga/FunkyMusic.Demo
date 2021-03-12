using System.Collections.Generic;
using System.Linq;
using System.Net;
using FunkyMusic.Demo.Api.Dto.Assets;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Application.Constants;
using FunkyMusic.Demo.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FunkyMusic.Demo.Api.ResponseFormatters
{
    public interface IResponseFormatter<T> where T : class
    {
        IActionResult GetActionResult(Result<T> result);
    }

    public class GetArtistByNameResponseFormatter : IResponseFormatter<SearchArtistByNameResponseDto>
    {
        public IActionResult GetActionResult(Result<SearchArtistByNameResponseDto> result)
        {
            if (!result.Status)
            {
                return GetErrorResponse(result);
            }

            return new OkObjectResult(result.Data);
        }

        private IActionResult GetErrorResponse(Result<SearchArtistByNameResponseDto> result)
        {
            var errorCode = result.ErrorCode;

            ErrorResponse errorResponse = null;
            var statusCode = HttpStatusCode.BadRequest;

            switch (errorCode)
            {
                case Domain.Constants.ErrorCodes.ArtistSearchExternalError:
                case Domain.Constants.ErrorCodes.ArtistSearchInternalError:
                    errorResponse = new ErrorResponse
                    {
                        ErrorCode = Domain.Constants.ErrorCodes.ArtistSearchExternalError,
                        Errors = new List<ErrorMessage>
                        {
                            new ErrorMessage {Field = "", Message = "Error occured when searching for artist."}
                        }
                    };

                    statusCode = HttpStatusCode.InternalServerError;
                    break;

                case Domain.Constants.ErrorCodes.ValidationError:
                    errorResponse = new ErrorResponse
                    {
                        ErrorCode = Domain.Constants.ErrorCodes.ValidationError,
                        Errors = result.Validation.Errors.Select(x => new ErrorMessage
                        {
                            Field = x.PropertyName,
                            Message = x.ErrorMessage
                        }).ToList()
                    };

                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case ErrorCodes.ArtistNotFound:
                    errorResponse = new ErrorResponse
                    {
                        ErrorCode = ErrorCodes.ArtistNotFound,
                        Errors = new List<ErrorMessage>
                        {
                            new ErrorMessage {Field = "Artist", Message = "Artist not found."}
                        }
                    };
                    statusCode = HttpStatusCode.NotFound;
                    break;
            }

            return new ObjectResult(errorResponse)
            {
                StatusCode = (int)(statusCode)
            };
        }
    }
}