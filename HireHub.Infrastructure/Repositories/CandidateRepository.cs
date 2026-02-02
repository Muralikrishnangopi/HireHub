using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireHub.Infrastructure.Repositories;

public class CandidateRepository : GenericRepository<Candidate>,  ICandidateRepository
{
    private new readonly HireHubDbContext _context;

    public CandidateRepository(HireHubDbContext context) : base(context)
    {
        _context = context;
    }


    #region DQL

    public async Task<int> CountCandidatesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Candidates.CountAsync(cancellationToken);
    }

    public async Task<int> CountByDriveStatusAsync(CandidateStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.DriveCandidates
            .Where(dc => dc.Status == status)
            .CountAsync(cancellationToken);
    }

    public async Task<List<Candidate>> GetAllAsync(CandidateFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Candidates.Select(u => u);

        if (filter.ExperienceLevel != null)
            query = query
                .Where(c => c.ExperienceLevel == filter.ExperienceLevel);

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

    public async Task<bool> IsCandidateWithEmailOrPhoneExist(string email, string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Candidates.AnyAsync(e => e.Email == email || e.Phone == phone, cancellationToken);
    }

    #endregion

    #region DML

    public async Task BulkInsertAsync(List<Candidate> candidates, CancellationToken cancellationToken = default)
    {
        await _context.Candidates.AddRangeAsync(candidates, cancellationToken);
    }

    #endregion

    #region Private Methods



    #endregion
}
