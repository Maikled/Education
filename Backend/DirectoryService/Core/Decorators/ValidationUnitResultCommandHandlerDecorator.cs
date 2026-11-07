using Contracts.Errors;
using Core.Interfaces;
using CSharpFunctionalExtensions;
using FluentValidation;

namespace Core.Decorators
{
    internal class ValidationUnitResultCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        private readonly ICommandHandler<TCommand> _next;

        public ValidationUnitResultCommandHandlerDecorator(IEnumerable<IValidator<TCommand>> validators, ICommandHandler<TCommand> next)
        {
            _validators = validators;
            _next = next;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await _next.HandlerAsync(command, cancellationToken);
            }

            var validationResults = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(command, cancellationToken)));

            var validationFailedResults = validationResults.Where(p => !p.IsValid).ToList();
            if (validationFailedResults.Count != 0)
            {
                return AppError.FromErrors([.. validationFailedResults.SelectMany(p => p.Errors.Select(e => AppError.Validation(e.ErrorCode, e.ErrorMessage)))]);
            }

            return await _next.HandlerAsync(command, cancellationToken);
        }
    }
}
