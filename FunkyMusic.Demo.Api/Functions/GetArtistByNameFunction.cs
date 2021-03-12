﻿using System.Threading.Tasks;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Api.Extensions;
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

        public GetArtistByNameFunction(IMediator mediator, ILogger<GetArtistByNameFunction> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName(nameof(GetArtistByNameFunction))]
        [OpenApiOperation("GetArtistByName", "FunkyMusic", Summary = "Get artist by name.", Description = "This will get the artist by connecting to the third party API.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("correlationId", In = ParameterLocation.Header, Required = true, Description = "The correlaion id of the operation.")]
        [OpenApiParameter("artistName", In = ParameterLocation.Path, Required = true, Description = "The name of the artist.")]
        public async Task<IActionResult> SearchAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "music/artist/{artistName}")]
            HttpRequest request, string artistName)
        {
            var correlationId = request.GetHeaderValue("correlationId");
            var searchArtistRequestDto = new SearchArtistByNameRequestDto
            {
                CorrelationId = correlationId,
                ArtistName = artistName
            };

            var operation = await _mediator.Send(searchArtistRequestDto);

            if (operation.Status)
            {
                return new OkResult();
            }

            return new BadRequestObjectResult(operation);
        }
    }
}