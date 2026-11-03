using Contracts.Errors;
using CSharpFunctionalExtensions;
using System.Data;

namespace Core.Interfaces
{
    public interface ITransactionManager
    {
        public Task<Result<IDbTransaction, AppError>> BeginTransactionAsync(CancellationToken cancellationToken);
        public Task<UnitResult<AppError>> SaveTransactionAsync(IDbTransaction transaction, CancellationToken cancellationToken);
    }
}
