using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Shared.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireHub.Infrastructure.Repositories;

public class RoundRepository : GenericRepository<Round>, IRoundRepository
{
    private new readonly HireHubDbContext _context;

    public RoundRepository(HireHubDbContext context) : base(context)
    {
        _context = context;
    }


    #region DQL

    public async Task<int> CountInterviewsAsync(RoundStatus? roundStatus,CancellationToken cancellationToken = default)
    {
        var query = _context.Rounds.Select(e => e);
        if (roundStatus != null)
            query = query.Where(r => r.Status == roundStatus);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<Round>> GetAllAsync(RoundFilter filter, CancellationToken cancellationToken = default)
    {
        var dQuery = _context.Drives.Select(e => e);
        if (filter.DriveId != null)
            dQuery = dQuery.Where(dm => dm.DriveId == filter.DriveId);
        dQuery = dQuery
            .OrderByDescending(d => d.DriveDate)
            .ThenByDescending(d => d.CreatedDate);

        var dmQuery = dQuery.SelectMany(d => d.DriveMembers);
        if (filter.UserId != null)
            dmQuery = dmQuery.Where(dm => dm.UserId == filter.UserId);

        var rQuery = dmQuery.SelectMany(e => e.InterviewedPanels);
        if (filter.RoundType != null)
            rQuery = rQuery.Where(r => r.RoundType == filter.RoundType);
        if (filter.RoundStatus != null)
            rQuery = rQuery.Where(r => r.Status == filter.RoundStatus);
        if (filter.RoundResult != null)
            rQuery = rQuery.Where(r => r.Result == filter.RoundResult);

        if (filter.PageNumber != null && filter.PageSize != null)
        {
            var pageNumber = (int)filter.PageNumber;
            var pageSize = (int)filter.PageSize;
            rQuery = rQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        return await rQuery.ToListAsync(cancellationToken);
    }

    public async Task<List<RoundDTO>> GetAllAsDtoAsync(RoundFilter filter, CancellationToken cancellationToken = default)
    {
        var dQuery = _context.Drives.Select(e => e);
        if (filter.DriveId != null)
            dQuery = dQuery.Where(dm => dm.DriveId == filter.DriveId);
        dQuery = dQuery
            .OrderByDescending(d => d.DriveDate)
            .ThenByDescending(d => d.CreatedDate);

        var dmQuery = dQuery.SelectMany(d => d.DriveMembers);
        if (filter.UserId != null)
            dmQuery = dmQuery.Where(dm => dm.UserId == filter.UserId);

        var rQuery = dmQuery.SelectMany(e => e.InterviewedPanels);
        if (filter.RoundType != null)
            rQuery = rQuery.Where(r => r.RoundType == filter.RoundType);
        if (filter.RoundStatus != null)
            rQuery = rQuery.Where(r => r.Status == filter.RoundStatus);
        if (filter.RoundResult != null)
            rQuery = rQuery.Where(r => r.Result == filter.RoundResult);

        if (filter.PageNumber != null && filter.PageSize != null)
        {
            var pageNumber = (int)filter.PageNumber;
            var pageSize = (int)filter.PageSize;
            rQuery = rQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        return await rQuery
            .Select(e => new RoundDTO
            {
                RoundId = e.RoundId,
                DriveId = e.Interviewer!.DriveId,
                DriveName = e.Interviewer.Drive!.DriveName,
                DriveDate = e.Interviewer.Drive.DriveDate,
                DriveStatus = e.Interviewer.Drive.Status.ToString(),
                CandidateId = e.DriveCandidate!.CandidateId,
                CandidateName = e.DriveCandidate.Candidate!.FullName,
                CandidateEmail = e.DriveCandidate.Candidate.Email,
                UserId = e.Interviewer.UserId,
                UserName = e.Interviewer.User!.FullName,
                UserEmail = e.Interviewer.User.Email,
                Type = e.RoundType.ToString(),
                RoundStatus = e.Status.ToString(),
                RoundResult = e.Result.ToString(),
                FeedbackId = e.FeedbackId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<RoundDTO?> GetByIdAsDtoAsync(int? roundId, CancellationToken cancellationToken = default)
    {
        return await _context.Rounds
            .Where(e =>  e.RoundId == roundId)
            .Select(e => new RoundDTO
            {
                RoundId = e.RoundId,
                DriveId = e.Interviewer!.DriveId,
                DriveName = e.Interviewer.Drive!.DriveName,
                DriveDate = e.Interviewer.Drive.DriveDate,
                DriveStatus = e.Interviewer.Drive.Status.ToString(),
                CandidateId = e.DriveCandidate!.CandidateId,
                CandidateName = e.DriveCandidate.Candidate!.FullName,
                CandidateEmail = e.DriveCandidate.Candidate.Email,
                UserId = e.Interviewer.UserId,
                UserName = e.Interviewer.User!.FullName,
                UserEmail = e.Interviewer.User.Email,
                Type = e.RoundType.ToString(),
                RoundStatus = e.Status.ToString(),
                RoundResult = e.Result.ToString(),
                FeedbackId = e.FeedbackId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public bool IsHrInterviewerForCandidateOnDrive(int userId, int candidateId, int driveId)
    {
        return _context.Rounds
            .Where(e => e.DriveCandidate!.DriveId == driveId && e.DriveCandidate.CandidateId == candidateId)
            .Where(e => e.Interviewer!.DriveId == driveId && e.Interviewer.UserId == userId)
            .Where(e => e.RoundType == RoundType.Hr)
            .Any();
    }

    public bool IsInterviewerForRound(int userId, int roundId)
    {
        return _context.Rounds
            .Where(e => e.RoundId == roundId)
            .Where(e => e.Interviewer!.UserId == userId)
            .Any();
    }

    public async Task<Round?> GetRoundByIdWithDetails(int roundId, CancellationToken cancellationToken = default)
    {
        return await _context.Rounds
            .Include(e => e.Interviewer)
            .Include(e => e.DriveCandidate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Round>> GetRoundsForDriveCandidate(int driveCandidateId, CancellationToken cancellationToken = default)
    {
        return _context.Rounds
            .Where(e => e.DriveCandidateId == driveCandidateId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DriveCandidate>> GetAllDriveCandidates(int driveId)
{
    var driveCandidates = await _context.DriveCandidates
                            .Where(dc => dc.DriveId == driveId)
                            .ToListAsync();
    return driveCandidates;
}

public async Task<List<DriveMember>> GetAllDriveMember(int driveId)
{
    return await _context.DriveMembers
        .Where(dm => dm.DriveId == driveId && dm.RoleId == 3)
        .ToListAsync();
}
public async Task<Round?> GetOldInterviewer(int roundId, int oldInterviewerId)
{
    return await _context.Rounds
        .Include(r => r.DriveCandidate)
        .FirstOrDefaultAsync(r =>
            r.RoundId == roundId &&
            r.InterviewerId == oldInterviewerId);
}



public async Task AssignNewInterview(Round round, int newInterviewerUserId)
{
    var newPanel = await _context.DriveMembers.FirstOrDefaultAsync(dm =>
        dm.DriveMemberId == newInterviewerUserId &&
        dm.RoleId == 3 &&
        dm.DriveId == round.DriveCandidate.DriveId);

    if (newPanel == null)
        throw new Exception("Invalid panel interviewer");

    round.InterviewerId = newPanel.DriveMemberId;
}
    #endregion

    #region DML



    #endregion

    #region Private Methods



    #endregion
}
