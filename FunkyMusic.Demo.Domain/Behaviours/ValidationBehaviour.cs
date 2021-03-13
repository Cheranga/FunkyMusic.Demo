using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FunkyMusic.Demo.Domain.Constants;
using FunkyMusic.Demo.Domain.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FunkyMusic.Demo.Domain.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : IValidatable
    {
        private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest> _validator;

        public ValidationBehaviour(IValidator<TRequest> validator, ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
        {
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse>> next)
        {
            if (_validator == null)
            {
                return await next();
            }

            var requestType = typeof(TRequest).Name;

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid)
            {
                _logger.LogInformation("Validation successful for {correlationId} in {dtoRequest}", request.CorrelationId, requestType);

                var operation = await next();
                return operation;
            }

            var errorMessage = string.Join(", ", validationResult.ToErrorMessage());
            _logger.LogWarning("Validation error occured for {correlationId} in {dtoRequest} with message: {errorMessage}", request?.CorrelationId,  requestType, errorMessage);
            return Result<TResponse>.Failure(ErrorCodes.ValidationError, validationResult);
        }
    }
}