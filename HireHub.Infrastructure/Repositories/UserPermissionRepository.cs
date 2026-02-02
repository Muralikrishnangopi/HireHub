using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireHub.Infrastructure.Repositories;

public class UserPermissionRepository : GenericRepository<UserPermission>, IUserPermissionRepository
{
    private new readonly HireHubDbContext _context;

    public UserPermissionRepository(HireHubDbContext context) : base(context)
    {
        _context = context;
    }


    #region DQL

    public async Task<UserPermission> GetUserPermissionAsync(int userId, string userAction)
    {
        return await _context.UserPermissions
            .FirstAsync(x => x.UserId == userId && x.Action == userAction);
    }

    #endregion

    #region DML



    #endregion

    #region Private Methods



    #endregion
}