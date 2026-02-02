using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Repositories;

namespace HireHub.Infrastructure.Repositories;

public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
{
    private new readonly HireHubDbContext _context;

    public FeedbackRepository(HireHubDbContext context) : base(context)
    {
        _context = context;
    }


    #region DQL



    #endregion

    #region DML



    #endregion

    #region Private Methods



    #endregion
}