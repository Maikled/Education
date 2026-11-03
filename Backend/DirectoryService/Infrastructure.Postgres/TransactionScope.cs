using Contracts.Errors;
using Core.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres
{
    internal partial class TransactionScope : ITransactionScope
    {
        private readonly IDbContextTransaction _transaction;
        private readonly ILogger _logger;

        public TransactionScope(IDbContextTransaction transaction, ILogger logger)
        {
            _transaction = transaction;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> CommitAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _transaction.CommitAsync(cancellationToken);

                return UnitResult.Success<AppError>();
            }
            catch(Exception ex)
            {
                LogCommitTransactionFailed(ex.Message);
                return AppError.Internal("database.commit.transaction", "Failed commit database transaction");
            }
        }

        public Task RollbackAsync(CancellationToken cancellationToken) => _transaction.RollbackAsync(cancellationToken);

        // EF откатывает незакоммиченную транзакцию при освобождении и сбрасывает Database.CurrentTransaction
        public ValueTask DisposeAsync() => _transaction.DisposeAsync();

        [LoggerMessage(Level = LogLevel.Error, Message = "Database transaction commit failed: {errorMessage}")]
        private partial void LogCommitTransactionFailed(string errorMessage);
    }
}
