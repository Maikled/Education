using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Core.Interfaces
{
    public interface ITransactionManager
    {
        public Task<Result<ITransactionScope, AppError>> BeginTransactionAsync(CancellationToken cancellationToken);
        public Task<UnitResult<AppError>> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
