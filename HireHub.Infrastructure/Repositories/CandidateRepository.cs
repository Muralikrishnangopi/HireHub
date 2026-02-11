using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Shared.Common.Exceptions;
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

        query = filter.IsLatestFirst == null ? query.OrderBy(u => u.FullName).ThenByDescending(u => u.CreatedDate) :
            filter.IsLatestFirst == true ? query.OrderByDescending(u => u.CreatedDate).ThenBy(u => u.FullName) :
            query.OrderBy(u => u.CreatedDate).ThenBy(u => u.FullName);

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

    public async Task<List<Candidate>> GetCandidatesByUserIdAsync(int userId)
    {
        return await _context.DriveMembers
            .Where(dm=>dm.UserId==userId&&dm.RoleId==3)
            .Join(
                _context.Rounds,
                dm => dm.DriveMemberId,
                r => r.InterviewerId,
                (dm, r) => r.DriveCandidate
            )
            .Select(dc=>dc.Candidate!)
            .Distinct()
            .ToListAsync();
    }

    public async Task<DriveCandidate?> GetValidDriveCandidateForAttendance(int driveId, int candidateId, int currentUserId)
    {
        var driveCandidate = await _context.DriveCandidates
        .Include(dc => dc.Drive)
        .Where(dc =>
            dc.DriveId == driveId &&
            dc.CandidateId == candidateId &&
            dc.Drive != null &&
            dc.Drive.DriveDate.Date == DateTime.Today &&
            dc.Drive.Status == DriveStatus.Started &&
            _context.DriveMembers.Any(dm =>
                dm.DriveId == driveId &&
                dm.UserId == currentUserId
            )
        )
        .FirstOrDefaultAsync();

        return driveCandidate;
    }

    public async Task CreateReassignmentAsync(
        int driveId,
        int candidateId,
        int previousUserId,
        int newUserId,
        bool requireApproval,
        int requestedBy)
    {
        var driveCandidate = await _context.DriveCandidates
            .Include(dc => dc.Rounds)
            .FirstOrDefaultAsync(dc =>
                dc.DriveId == driveId &&
                dc.CandidateId == candidateId);

        if (driveCandidate == null)
            throw new CommonException("Candidate not found in drive");

        var previousPanel = await _context.DriveMembers.FirstOrDefaultAsync(dm =>
            dm.DriveId == driveId &&
            dm.UserId == previousUserId &&
            dm.RoleId == 3);

        var newPanel = await _context.DriveMembers.FirstOrDefaultAsync(dm =>
            dm.DriveId == driveId &&
            dm.UserId == newUserId &&
            dm.RoleId == 3);

        if (previousPanel == null || newPanel == null)
            throw new CommonException("Invalid panel member");

        var round = driveCandidate.Rounds
            .FirstOrDefault(r => r.InterviewerId == previousPanel.DriveMemberId);

        if (round == null)
            throw new CommonException("Round not found");

        var reassignment = new CandidateReassignment
        {
            DriveCandidateId = driveCandidate.DriveCandidateId,
            PreviousUserId = previousPanel.DriveMemberId,
            NewUserId = newPanel.DriveMemberId,
            RequestedBy = requestedBy,
            RequireApproval = requireApproval,
            RequestedDate = DateTime.Now
        };

        _context.CandidateReassignments.Add(reassignment);

        //if (!requireApproval)
        //{
        //    round.InterviewerId = newPanel.DriveMemberId;
        //    reassignment.ApprovedBy = requestedBy;
        //    reassignment.ApprovedDate = DateTime.Now;
        //}
        round.InterviewerId = reassignment.NewUserId;
        await _context.SaveChangesAsync();
    }
    #endregion

    #region Private Methods



    #endregion
}
