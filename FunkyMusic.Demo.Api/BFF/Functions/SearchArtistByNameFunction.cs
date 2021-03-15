using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FunkyMusic.Demo.Api.BFF.Responses;
using FunkyMusic.Demo.Api.Dto.Assets;
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

namespace FunkyMusic.Demo.Api.BFF.Functions
{
    public class SearchArtistByNameFunction
    {
        private readonly IMediator _mediator;
        private readonly IResponseFormatter<SearchArtistByNameResponseDto> _artistResponseFormatter;

        public SearchArtistByNameFunction(IMediator mediator, IResponseFormatter<SearchArtistByNameResponseDto> artistResponseFormatter)
        {
            _mediator = mediator;
            _artistResponseFormatter = artistResponseFormatter;
        }

        [FunctionName(nameof(SearchArtistByNameFunction))]
        [OpenApiOperation("SearchArtistByName", "FunkyMusic", Summary = "Bff related artist search by name.", Description = "This is a BFF function where it will return multiple artists if matching, else it will return all the recordings associated with the artist.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("correlationId", In = ParameterLocation.Header, Required = true, Description = "The correlaion id of the operation.")]
        [OpenApiParameter("name", In = ParameterLocation.Query, Required = true, Description = "The name of the artist.")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(SearchArtistByNameResponse), Summary = "The artist and the records associated.", Description = "The artist and the records associated.")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(SearchArtistByNameResponseDto), Summary = "All the artists which matches the search.", Description = "All the artists which matches the search.")]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse), Summary = "The artist search request is invalid.", Description = "The artist search request is invalid.")]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, "application/json", typeof(ErrorResponse), Summary = "The artist search encountered an error.", Description = "The artist search encountered an error.")]
        public async Task<IActionResult> SearchAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "myapp/music/artist")]
            HttpRequest request)
        {
            var correlationId = request.GetHeaderValue("correlationId");
            var artistName = request.GetQueryStringValue("name");

            var searchArtistRequest = new SearchArtistByNameRequestDto
            {
                CorrelationId = correlationId,
                ArtistName = artistName
            };

            var artistSearchOperation = await _mediator.Send(searchArtistRequest);
            if (!artistSearchOperation.Status)
            {
                return _artistResponseFormatter.GetActionResult(artistSearchOperation);
            }

            var artists = artistSearchOperation.Data.Artists;
            if (artists.Count > 1)
            {
                return _artistResponseFormatter.GetActionResult(artistSearchOperation);
            }

            var artistId = artists.First().ArtistId;
            var getRecordsForArtist = new SearchRecordsForArtistByIdRequestDto
            {
                CorrelationId = correlationId,
                ArtistId = artistId
            };

            var getRecordsForArtistOperation = await _mediator.Send(getRecordsForArtist);
            var records = getRecordsForArtistOperation.Status ? getRecordsForArtistOperation.Data.Records : new List<RecordDto>();

            var response = new SearchArtistByNameResponse
            {
                ArtistId = artistId,
                ArtistName = artistName,
                Records = records
            };

            return new OkObjectResult(response);
        }

    }
}
