using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest>? _validator;
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;

        public ValidatorBehavior(ILogger<ValidatorBehavior<TRequest, TResponse>> logger, IValidator<TRequest>? validator = null)
        {
            _validator = validator;
            _logger = logger;
        }


        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validator is null)
                return await next();

            _logger.LogInformation($"Handling {typeof(TRequest).Name}");

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            return validationResult.IsValid ? await next() : throw new ValidationException(validationResult.Errors);
        }
    }
}
