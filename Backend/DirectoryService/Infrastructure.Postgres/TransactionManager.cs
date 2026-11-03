using Contracts.Errors;
using Core.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Data;

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

        public async Task<Result<IDbTransaction, AppError>> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                var databaseTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                return databaseTransaction.GetDbTransaction();
            }
            catch(Exception ex)
            {
                LogBeginTransactionFailed(ex.Message);
                return AppError.Internal("database.begin.transaction", "Failed begin database transaction");
            }
        }

        public async Task<UnitResult<AppError>> SaveTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                return UnitResult.Success<AppError>();
            }
            catch(Exception ex)
            {
                LogSaveTransactionFailed(ex.Message);
                return AppError.Internal("database.save.transaction", "Failed save database transaction");
            }
        }

        [LoggerMessage(Level = LogLevel.Error, Message = "Database transaction begin failed: {errorMessage}")]
        private partial void LogBeginTransactionFailed(string errorMessage);

        [LoggerMessage(Level = LogLevel.Error, Message = "Database transaction save failed: {errorMessage}")]
        private partial void LogSaveTransactionFailed(string errorMessage);
    }
}
