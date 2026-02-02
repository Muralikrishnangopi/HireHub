namespace HireHub.Core.Data.Interface;

public interface ISaveRepository
{
    int SaveChanges();
    Task<int> SaveChangesAsync();
}
