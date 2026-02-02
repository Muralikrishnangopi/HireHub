using HireHub.Core.Data.Interface;

namespace HireHub.Infrastructure.Repositories;

public class SaveRepository : ISaveRepository
{
    private readonly HireHubDbContext _context;

    public SaveRepository(HireHubDbContext context)
    {
        _context = context;
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

}
