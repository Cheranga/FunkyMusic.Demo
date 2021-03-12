using FluentValidation;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Domain.Validators;

namespace FunkyMusic.Demo.Application.Validators
{
    internal class GetArtistByNameRequestValidator : ModelValidatorBase<GetArtistByNameRequest>
    {
        public GetArtistByNameRequestValidator()
        {
            RuleFor(x => x.CorrelationId).NotNull().NotEmpty();
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}