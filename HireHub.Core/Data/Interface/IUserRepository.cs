using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Interface;

namespace HireHub.Core.Data.Interface;

public interface IUserRepository : IGenericRepository<User>
{
    #region DQL

    Task<User?> GetByEmailAsync(string emailId, CancellationToken cancellationToken = default);

    Task<int> CountUsersAsync(CancellationToken cancellationToken = default);

    Task<int> CountUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    Task<List<User>> GetAllAsync(UserFilter filter, CancellationToken cancellationToken = default);

    Task<bool> IsUserWithEmailOrPhoneExist(string email, string phone);

    #endregion

    #region DML



    #endregion
}
