﻿using FunkyMusic.Demo.Api.Dto.Responses;
using FunkyMusic.Demo.Domain;
using MediatR;

namespace FunkyMusic.Demo.Api.Dto.Requests
{
    public class SearchRecordsForArtistByIdRequestDto : IRequest<Result<SearchRecordsForArtistByIdResponseDto>>, IValidatable
    {
        public string CorrelationId { get; set; }
        public string ArtistId { get; set; }
    }
}