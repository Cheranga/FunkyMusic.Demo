using FluentValidation;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Domain.Validators;

namespace FunkyMusic.Demo.Application.Validators
{
    public class SearchRecordsForArtistByIdRequestValidator : ModelValidatorBase<SearchRecordsForArtistByIdRequest>
    {
        public SearchRecordsForArtistByIdRequestValidator()
        {
            RuleFor(x => x.CorrelationId).NotNull().NotEmpty();
            RuleFor(x => x.ArtistId).NotNull().NotEmpty();
        }
    }
}