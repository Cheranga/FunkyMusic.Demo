using FluentValidation;
using FunkyMusic.Demo.Application.Dto;
using FunkyMusic.Demo.Domain.Validators;

namespace FunkyMusic.Demo.Application.Validators
{
    public class SearchArtistByNameRequestValidator : ModelValidatorBase<SearchArtistByNameRequest>
    {
        public SearchArtistByNameRequestValidator()
        {
            RuleFor(x => x.CorrelationId).NotNull().NotEmpty();
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}