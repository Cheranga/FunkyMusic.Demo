using FluentValidation;
using FunkyMusic.Demo.Application.Requests;
using FunkyMusic.Demo.Domain.Validators;

namespace FunkyMusic.Demo.Application.Validators
{
    internal class GetRecordsForArtistByIdRequestValidator : ModelValidatorBase<GetRecordsForArtistByIdRequest>
    {
        public GetRecordsForArtistByIdRequestValidator()
        {
            RuleFor(x => x.CorrelationId).NotNull().NotEmpty();
            RuleFor(x => x.ArtistId).NotNull().NotEmpty();
        }
    }
}