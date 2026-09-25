using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Extensions
{
    public static class TransactionServiceExtensions
    {
        public static async Task ExecuteTransactionalAsync(
            this DbContext dbContext,
            Func<Task> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy(); // PostgreSQL execution strategy

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(isolationLevel);
                try
                {
                    await operation(); // Execute business logic
                    await dbContext.SaveChangesAsync(); // Save changes to the database
                    await transaction.CommitAsync(); // Commit transaction
                }
                catch
                {
                    await transaction.RollbackAsync(); // Rollback on failure
                    dbContext.ChangeTracker.Clear();
                    throw;
                }
            });
        }
    }
}
