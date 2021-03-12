using FluentValidation;
using FunkyMusic.Demo.Api.Dto.Requests;
using FunkyMusic.Demo.Domain.Validators;

namespace FunkyMusic.Demo.Api.Validators
{
    public class SearchArtistByNameRequestDtoValidator : ModelValidatorBase<SearchArtistByNameRequestDto>
    {
        public SearchArtistByNameRequestDtoValidator()
        {
            RuleFor(x => x.CorrelationId).NotNull().NotEmpty();
            RuleFor(x => x.ArtistName).NotNull().NotEmpty();
        }
    }
}