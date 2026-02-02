using HireHub.Core.Data.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace HireHub.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly HireHubDbContext _context;

    public TransactionRepository(HireHubDbContext context)
    {
        _context = context;
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public void RollbackTransaction()
    {
        _context.Database.RollbackTransaction();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public void CommitTransaction()
    {
        _context.Database.CommitTransaction();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

}
