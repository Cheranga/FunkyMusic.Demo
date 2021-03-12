using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Api.Extensions;
using FunkyMusic.Demo.Api.ResponseFormatters;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace FunkyMusic.Demo.Api.Functions
{
    public class GetRecordsForArtistByIdFunction
    {
        private readonly IMediator _mediator;
        private readonly IResponseFormatter<SearchRecordsForArtistByIdResponseDto> _responseFormatter;

        public GetRecordsForArtistByIdFunction(IMediator mediator, IResponseFormatter<SearchRecordsForArtistByIdResponseDto> responseFormatter)
        {
            _mediator = mediator;
            _responseFormatter = responseFormatter;
        }

        [FunctionName(nameof(GetRecordsForArtistByIdFunction))]
        [OpenApiOperation("GetRecordsForArtistById", "FunkyMusic", Summary = "Get records for artist by id.", Description = "This will get the records for the artist id from the third party API.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("correlationId", In = ParameterLocation.Header, Required = true, Description = "The correlaion id of the operation.")]
        [OpenApiParameter("artistId", In = ParameterLocation.Path, Required = true, Description = "The id of the artist.")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(SearchRecordsForArtistByIdResponseDto), Summary = "The records belonging to the artist.", Description = "The records belonging to the artist.")]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse), Summary = "The record search for artist id is invalid.", Description = "The record search for artist id is invalid.")]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, "application/json", typeof(ErrorResponse), Summary = "The record search for artist id encountered an error.", Description = "The record search for artist id encountered an error.")]
        public async Task<IActionResult> SearchAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "records/artist/{artistId}")]
            HttpRequest request, string artistId)
        {
            var correlationId = request.GetHeaderValue("correlationId");
            var searchArtistRequestDto = new SearchRecordsForArtistByIdRequestDto
            {
                CorrelationId = correlationId,
                ArtistId = artistId
            };

            var operation = await _mediator.Send(searchArtistRequestDto);

            return _responseFormatter.GetActionResult(operation);
        }
    }
}
