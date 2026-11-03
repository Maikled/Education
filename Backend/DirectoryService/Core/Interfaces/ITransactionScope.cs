using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Core.Interfaces
{
    public interface ITransactionScope : IAsyncDisposable
    {
        public Task<UnitResult<AppError>> CommitAsync(CancellationToken cancellationToken);
        public Task RollbackAsync(CancellationToken cancellationToken);
    }
}
