using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Domain;
using FunkyMusic.Demo.Domain.Models;
using MediatR;

namespace FunkyMusic.Demo.Infrastructure.Api.Handlers
{
    internal class SearchRecordsForArtistByIdRequestHandler : IRequestHandler<SearchRecordsForArtistByIdRequest, Result<List<Record>>>
    {
        public Task<Result<List<Record>>> Handle(SearchRecordsForArtistByIdRequest request, CancellationToken cancellationToken)
        {
            var records = Enumerable.Range(1, 10).Select(x => new Record
            {
                Id = x.ToString(),
                Length = x * 100,
                ReleaseDate = DateTime.Now.AddYears(-10).Year.ToString(),
                Title = $"Record{x}"
            }).ToList();

            return Task.FromResult(Result<List<Record>>.Success(records));
        }
    }
}