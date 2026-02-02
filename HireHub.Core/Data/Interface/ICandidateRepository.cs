using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Interface;

namespace HireHub.Core.Data.Interface;

public interface ICandidateRepository : IGenericRepository<Candidate>
{
    #region DQL

    Task<int> CountCandidatesAsync(CancellationToken cancellationToken = default);

    Task<int> CountByDriveStatusAsync(CandidateStatus status, CancellationToken cancellationToken = default);

    Task<List<Candidate>> GetAllAsync(CandidateFilter filter, CancellationToken cancellationToken = default);

    Task<bool> IsCandidateWithEmailOrPhoneExist(string email, string phone, CancellationToken cancellationToken = default);

    #endregion

    #region DML

    Task BulkInsertAsync(List<Candidate> candidates, CancellationToken cancellationToken = default);

    #endregion
}