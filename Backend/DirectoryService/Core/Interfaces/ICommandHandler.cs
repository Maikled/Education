using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Core.Interfaces
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        public Task<UnitResult<AppError>> HandlerAsync(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<TResponse, in TCommand> where TCommand : ICommand
    {
        public Task<Result<TResponse, AppError>> HandlerAsync(TCommand command, CancellationToken cancellationToken);
    }
}