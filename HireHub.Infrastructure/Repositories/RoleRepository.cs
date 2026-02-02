using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireHub.Infrastructure.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    private new readonly HireHubDbContext _context;

    public RoleRepository(HireHubDbContext context) : base(context)
    {
        _context = context;
    }


    #region DQL

    public Task<Role> GetByName(UserRole name)
    {
        return _context.Roles.FirstAsync(e => e.RoleName == name);
    }

    #endregion

    #region DML



    #endregion

    #region Private Methods



    #endregion
}
