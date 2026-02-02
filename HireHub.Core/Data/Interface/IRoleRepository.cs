using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Interface;

namespace HireHub.Core.Data.Interface;

public interface IRoleRepository : IGenericRepository<Role>
{
    #region DQL

    Task<Role> GetByName(UserRole name);

    #endregion

    #region DML



    #endregion
}
