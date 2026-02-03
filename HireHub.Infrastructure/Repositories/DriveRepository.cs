using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Shared.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireHub.Infrastructure.Repositories;

public class DriveRepository : GenericRepository<Drive>, IDriveRepository
{
    private new readonly HireHubDbContext _context;

    public DriveRepository(HireHubDbContext context) : base(context)
    {
        _context = context;
    }


    #region DQL

    public async Task<int> CountDrivesAsync(DriveStatus? driveStatus, CancellationToken cancellationToken = default)
    {
        var query = _context.Drives.Select(e => e);
        if (driveStatus != null)
            query = query.Where(d => d.Status == driveStatus);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<Drive>> GetAllAsync(DriveFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Drives.Include(e => e.Creator).Select(u => u);

        if (filter.Status != null)
            query = query
                .Where(d => d.Status == filter.Status);

        if (filter.CreatorEmail != null)
            query = query
                .Where(d => d.Creator!.Email == filter.CreatorEmail);

        if (filter.TechnicalRounds != null)
            query = query
                .Where(d => d.TechnicalRounds == filter.TechnicalRounds);

        if (!filter.IncludePastDrives)
            query = query
                .Where(d => d.DriveDate >= DateTime.Today);

        if (filter.StartDate != null)
            query = query
                .Where(d => d.DriveDate >= filter.StartDate);

        if (filter.EndDate != null)
            query = query
                .Where(d => d.DriveDate <= filter.EndDate);

        if (filter.PageNumber != null && filter.PageSize != null)
        {
            var pageNumber = (int)filter.PageNumber;
            var pageSize = (int)filter.PageSize;
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        query = filter.IsLatestFirst ?
            query.OrderByDescending(d => d.DriveDate).ThenByDescending(d => d.CreatedDate) :
            query.OrderBy(u => u.DriveDate).ThenBy(d => d.CreatedDate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserAssignedInAnyActiveDriveOnDateAsync(int userId, DateTime driveDate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.DriveMembers
            .Where(dm => dm.UserId == userId)
            .Include(dm => dm.Drive)
            .AnyAsync(
                dm => dm.Drive!.DriveDate.Date == driveDate.Date && 
                    dm.Drive.Status != DriveStatus.Completed && dm.Drive.Status != DriveStatus.Cancelled,
                cancellationToken
            );
    }

    public async Task<Drive?> GetDriveWithConfigAsync(int driveId, CancellationToken cancellationToken = default)
    {
        return await _context.Drives
            .Where(d => d.DriveId == driveId)
            .Include(e => e.DriveRoleConfigurations)
            .Include(e => e.PanelVisibilitySettings)
            .Include(e => e.NotificationSettings)
            .Include(e => e.FeedbackConfiguration)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsDriveWithNameExist(string driveName, CancellationToken cancellationToken = default)
    {
        return await _context.Drives.AnyAsync(e => e.DriveName == driveName, cancellationToken);
    }

    public async Task<Drive?> GetDriveWithMembersAsync(int driveId, CancellationToken cancellationToken = default)
    {
        return await _context.Drives
            .Where(d => d.DriveId == driveId)
            .Include(e => e.DriveMembers)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Drive?> GetDriveWithCandidatesAsync(int driveId, CancellationToken cancellationToken = default)
    {
        return await _context.Drives
            .Where(d => d.DriveId == driveId)
            .Include(e => e.DriveCandidates)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Drive?> GetDriveWithCandidatesAsync(int driveId, List<int> candidateIds, CancellationToken cancellationToken = default)
    {
        var drive = await _context.Drives
            .Where(d => d.DriveId == driveId)
            .FirstOrDefaultAsync();
        if (drive != null)
            drive.DriveCandidates = await _context.DriveCandidates
                .Where(dc => dc.DriveId == driveId && candidateIds.Contains(dc.CandidateId))
                .ToListAsync();
        return drive;
    }

    public async Task<List<DriveMember>> GetDriveMembersWithDetailsAsync(DriveMemberFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.DriveMembers
            .Include(dm => dm.Drive)
            .Include(dm => dm.User)
            .Include(dm => dm.Role)
            .Select(dm => dm);

        if (filter.DriveId != null)
            query = query
                .Where(dm => dm.DriveId == filter.DriveId);

        if (filter.UserId != null)
            query = query
                .Where(dm => dm.UserId == filter.UserId);

        if (filter.Role != null)
            query = query
                .Where(dm => dm.Role!.RoleName == filter.Role);

        if (filter.DriveStatus != null)
            query = query
                .Where(dm => dm.Drive!.Status == filter.DriveStatus);

        if (!filter.IncludePastDrives)
            query = query
                .Where(dm => dm.Drive!.DriveDate >= DateTime.Today);

        if (filter.StartDate != null)
            query = query
                .Where(dm => dm.Drive!.DriveDate >= filter.StartDate);

        if (filter.EndDate != null)
            query = query
                .Where(dm => dm.Drive!.DriveDate <= filter.EndDate);

        if (filter.PageNumber != null && filter.PageSize != null)
        {
            var pageNumber = (int)filter.PageNumber;
            var pageSize = (int)filter.PageSize;
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        query = filter.IsLatestFirst ?
            query.OrderByDescending(dm => dm.Drive!.DriveDate).ThenByDescending(dm => dm.Drive!.CreatedDate) :
            query.OrderBy(dm => dm.Drive!.DriveDate).ThenBy(dm => dm.Drive!.CreatedDate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<DriveMemberDTO>> GetDriveMembersAsDtoAsync(DriveMemberFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.DriveMembers.Select(dm => dm);

        if (filter.DriveId != null)
            query = query
                .Where(dm => dm.DriveId == filter.DriveId);

        if (filter.UserId != null)
            query = query
                .Where(dm => dm.UserId == filter.UserId);

        if (filter.Role != null)
            query = query
                .Where(dm => dm.Role!.RoleName == filter.Role);

        if (filter.DriveStatus != null)
            query = query
                .Where(dm => dm.Drive!.Status == filter.DriveStatus);

        if (!filter.IncludePastDrives)
            query = query
                .Where(dm => dm.Drive!.DriveDate >= DateTime.Today);

        if (filter.StartDate != null)
            query = query
                .Where(dm => dm.Drive!.DriveDate >= filter.StartDate);

        if (filter.EndDate != null)
            query = query
                .Where(dm => dm.Drive!.DriveDate <= filter.EndDate);

        if (filter.PageNumber != null && filter.PageSize != null)
        {
            var pageNumber = (int)filter.PageNumber;
            var pageSize = (int)filter.PageSize;
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        query = filter.IsLatestFirst ?
            query.OrderByDescending(dm => dm.Drive!.DriveDate).ThenByDescending(dm => dm.Drive!.CreatedDate) :
            query.OrderBy(dm => dm.Drive!.DriveDate).ThenBy(dm => dm.Drive!.CreatedDate);

        return await query
            .Select(e => new DriveMemberDTO
            {
                DriveMemberId = e.DriveMemberId,
                DriveId = e.DriveId,
                DriveName = e.Drive!.DriveName,
                DriveDate = e.Drive.DriveDate,
                DriveStatus = e.Drive.Status.ToString(),
                UserId = e.UserId,
                UserName = e.User!.FullName,
                UserEmail = e.User.Email,
                RoleId = e.RoleId,
                RoleName = e.Role!.RoleName.ToString()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DriveCandidate>> GetDriveCandidatesWithDetailsAsync(DriveCandidateFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.DriveCandidates
            .Include(dc => dc.Drive)
            .Include(dc => dc.Candidate)
            .Select(dc => dc);

        if (filter.DriveId != null)
            query = query
                .Where(dc => dc.DriveId == filter.DriveId);

        if (filter.CandidateId != null)
            query = query
                .Where(dc => dc.CandidateId == filter.CandidateId);

        if (filter.CandidateStatus != null)
            query = query
                .Where(dc => dc.Status == filter.CandidateStatus);

        if (filter.DriveStatus != null)
            query = query
                .Where(dm => dm.Drive!.Status == filter.DriveStatus);

        if (!filter.IncludePastDrives)
            query = query
                .Where(dm => dm.Drive!.DriveDate >= DateTime.Today);

        if (filter.StartDate != null)
            query = query
                .Where(dm => dm.Drive!.DriveDate >= filter.StartDate);

        if (filter.EndDate != null)
            query = query
                .Where(dm => dm.Drive!.DriveDate <= filter.EndDate);

        if (filter.PageNumber != null && filter.PageSize != null)
        {
            var pageNumber = (int)filter.PageNumber;
            var pageSize = (int)filter.PageSize;
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        query = filter.IsLatestFirst ?
            query.OrderByDescending(dm => dm.Drive!.DriveDate).ThenByDescending(dm => dm.Drive!.CreatedDate) :
            query.OrderBy(dm => dm.Drive!.DriveDate).ThenBy(dm => dm.Drive!.CreatedDate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<DriveCandidateDTO>> GetDriveCandidatesAsDtoAsync(DriveCandidateFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.DriveCandidates.Select(dc => dc);

        if (filter.DriveId != null)
            query = query
                .Where(dc => dc.DriveId == filter.DriveId);

        if (filter.CandidateId != null)
            query = query
                .Where(dc => dc.CandidateId == filter.CandidateId);

        if (filter.CandidateStatus != null)
            query = query
                .Where(dc => dc.Status == filter.CandidateStatus);

        if (filter.DriveStatus != null)
            query = query
                .Where(dm => dm.Drive!.Status == filter.DriveStatus);

        if (!filter.IncludePastDrives)
            query = query
                .Where(dm => dm.Drive!.DriveDate >= DateTime.Today);

        if (filter.StartDate != null)
            query = query
                .Where(dm => dm.Drive!.DriveDate >= filter.StartDate);

        if (filter.EndDate != null)
            query = query
                .Where(dm => dm.Drive!.DriveDate <= filter.EndDate);

        if (filter.PageNumber != null && filter.PageSize != null)
        {
            var pageNumber = (int)filter.PageNumber;
            var pageSize = (int)filter.PageSize;
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        query = filter.IsLatestFirst ?
            query.OrderByDescending(dm => dm.Drive!.DriveDate).ThenByDescending(dm => dm.Drive!.CreatedDate) :
            query.OrderBy(dm => dm.Drive!.DriveDate).ThenBy(dm => dm.Drive!.CreatedDate);

        return await query
            .Select(e => new DriveCandidateDTO
            {
                DriveCandidateId = e.DriveCandidateId,
                CandidateId = e.CandidateId,
                CandidateName = e.Candidate!.FullName,
                CandidateEmail = e.Candidate.Email,
                DriveId = e.DriveId,
                DriveName = e.Drive!.DriveName,
                DriveDate = e.Drive.DriveDate,
                DriveStatus = e.Drive.Status.ToString(),
                CandidateStatus = e.Status.ToString(),
                StatusSetBy = e.StatusSetBy
            })
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region DML

    public void RemoveDriveMember(DriveMember driveMember)
    {
        _context.DriveMembers.Remove(driveMember);
    }


    public void RemoveDriveCandidate(DriveCandidate driveCandidate)
    {
        _context.DriveCandidates.Remove(driveCandidate);
    }

    #endregion

    #region Private Methods



    #endregion
}
