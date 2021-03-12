using FluentValidation;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Domain.Validators;

namespace FunkyMusic.Demo.Api.Validators
{
    public class SearchRecordsForArtistByIdRequestDtoValidator : ModelValidatorBase<SearchRecordsForArtistByIdRequestDto>
    {
        public SearchRecordsForArtistByIdRequestDtoValidator()
        {
            RuleFor(x => x.CorrelationId).NotNull().NotEmpty();
            RuleFor(x => x.ArtistId).NotNull().NotEmpty();
        }
    }
}