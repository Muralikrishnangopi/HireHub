using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireHub.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private new readonly HireHubDbContext _context;

    public UserRepository(HireHubDbContext context) : base(context)
    {
        _context = context;
    }


    #region DQL

    public async Task<User?> GetByEmailAsync(string emailId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(e => e.Email == emailId, cancellationToken);
    }

    public async Task<int> CountUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.CountAsync(cancellationToken);
    }

    public async Task<int> CountUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => r.RoleName == role)
            .Join(_context.Users, r => r.RoleId, u => u.RoleId, (r, u) => u)
            .CountAsync(cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(UserFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Include(u => u.Role).Select(u => u);

        if (filter.Role != null)
            query = query
                .Where(u => u.Role!.RoleName == filter.Role);

        if (filter.IsActive != null)
            query = query
                .Where(u => u.IsActive == filter.IsActive);

        if (filter.StartDate != null)
            query = query
                .Where(u => u.CreatedDate >= filter.StartDate);

        if (filter.EndDate != null)
            query = query
                .Where(u => u.CreatedDate <= filter.EndDate);

        if (filter.PageNumber != null && filter.PageSize != null)
        {
            var pageNumber = (int)filter.PageNumber;
            var pageSize = (int)filter.PageSize;
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        query = filter.IsLatestFirst ?
            query.OrderByDescending(u => u.CreatedDate) :
            query.OrderBy(u => u.CreatedDate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserWithEmailOrPhoneExist(string email, string phone)
    {
        return await _context.Users.AnyAsync(e => e.Email == email || e.Phone == phone);
    }

    #endregion

    #region DML



    #endregion

    #region Private Methods



    #endregion
}
