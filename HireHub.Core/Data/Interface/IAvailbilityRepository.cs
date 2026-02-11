using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.Data.Interface
{
    public interface IAvailbilityRepository:IGenericRepository<Availability>
    {

        #region DQL
        Task<List<User>> GetUserForDriveAsync(DateTime date, CancellationToken cancellationToken = default);
        #endregion
    }
}
