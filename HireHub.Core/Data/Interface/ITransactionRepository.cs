using Microsoft.EntityFrameworkCore.Storage;

namespace HireHub.Core.Data.Interface;

public interface ITransactionRepository
{
    IDbContextTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
    void RollbackTransaction();
    Task RollbackTransactionAsync();
    void CommitTransaction();
    Task CommitTransactionAsync();
}
