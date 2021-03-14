using System.Net;
using System.Threading.Tasks;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Api.Extensions;
using FunkyMusic.Demo.Api.ResponseFormatters;
using FunkyMusic.Demo.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace FunkyMusic.Demo.Api.Functions
{
    public class GetArtistByNameFunction
    {
        private readonly ILogger<GetArtistByNameFunction> _logger;
        private readonly IMediator _mediator;
        private readonly IResponseFormatter<SearchArtistByNameResponseDto> _responseFormatter;

        public GetArtistByNameFunction(IMediator mediator, IResponseFormatter<SearchArtistByNameResponseDto> responseFormatter, ILogger<GetArtistByNameFunction> logger)
        {
            _mediator = mediator;
            _responseFormatter = responseFormatter;
            _logger = logger;
        }

        [FunctionName(nameof(GetArtistByNameFunction))]
        [OpenApiOperation("GetArtistByName", "FunkyMusic", Summary = "Search artist by name.", Description = "This will get the artist by connecting to the third party API.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("correlationId", In = ParameterLocation.Header, Required = true, Description = "The correlaion id of the operation.")]
        [OpenApiParameter("name", In = ParameterLocation.Query, Required = true, Description = "The name of the artist.")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(SearchArtistByNameResponseDto), Summary = "The artist/s which matches the search.", Description = "The artist/s which matches the search.")]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse), Summary = "The artist search request is invalid.", Description = "The artist search request is invalid.")]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, "application/json", typeof(ErrorResponse), Summary = "The artist search encountered an error.", Description = "The artist search encountered an error.")]
        public async Task<IActionResult> SearchAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "music/search/artist")]
            HttpRequest request)
        {
            var correlationId = request.GetHeaderValue("correlationId");
            var artistName = request.GetQueryStringValue("name");
            var searchArtistRequestDto = new SearchArtistByNameRequestDto
            {
                CorrelationId = correlationId,
                ArtistName = artistName
            };

            var operation = await _mediator.Send(searchArtistRequestDto);

            return _responseFormatter.GetActionResult(operation);
        }
    }
}