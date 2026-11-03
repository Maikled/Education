using Contracts.Errors;
using Core.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres
{
    internal partial class TransactionManager : ITransactionManager
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TransactionManager> _logger;

        public TransactionManager(AppDbContext dbContext, ILogger<TransactionManager> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<ITransactionScope, AppError>> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                var databaseTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                return new TransactionScope(databaseTransaction, _logger);
            }
            catch(Exception ex)
            {
                LogBeginTransactionFailed(ex.Message);
                return AppError.Internal("database.begin.transaction", "Failed begin database transaction");
            }
        }

        public async Task<UnitResult<AppError>> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                return UnitResult.Success<AppError>();
            }
            catch(Exception ex)
            {
                LogSaveChangesFailed(ex.Message);
                return AppError.Internal("database.save.changes", "Failed save database changes");
            }
        }

        [LoggerMessage(Level = LogLevel.Error, Message = "Database transaction begin failed: {errorMessage}")]
        private partial void LogBeginTransactionFailed(string errorMessage);

        [LoggerMessage(Level = LogLevel.Error, Message = "Database save changes failed: {errorMessage}")]
        private partial void LogSaveChangesFailed(string errorMessage);
    }
}
