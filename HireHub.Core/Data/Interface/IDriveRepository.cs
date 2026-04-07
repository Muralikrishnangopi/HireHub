using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Shared.Persistence.Interface;

namespace HireHub.Core.Data.Interface;

public interface IDriveRepository : IGenericRepository<Drive>
{
    #region DQL
    Task<DriveCandidate?> GetDriveCandidateWithIdAsync(int driveCandidateId,CancellationToken cancellationToken=default);
    Task<int> CountDrivesAsync(DriveStatus? driveStatus, CancellationToken cancellationToken = default);

    Task<List<Drive>> GetAllAsync(DriveFilter filter, CancellationToken cancellationToken = default);

    Task<bool> IsUserAssignedInAnyActiveDriveOnDateAsync(int userId, DateTime driveDate, CancellationToken cancellationToken = default);
    Task<bool> IsUserAssignedWithDriveId(int userId, int driveId, CancellationToken cancellationToken = default);

    Task<Drive?> GetDriveWithConfigAsync(int driveId, CancellationToken cancellationToken = default);

    Task<bool> IsDriveWithNameExist(string driveName, CancellationToken cancellationToken = default);

    Task<Drive?> GetDriveWithMembersAsync(int driveId, CancellationToken cancellationToken = default);

    [Obsolete]
    Task<Drive?> GetDriveWithCandidatesAsync(int driveId, CancellationToken cancellationToken = default);

    Task<Drive?> GetDriveWithCandidatesAsync(int driveId, List<int> candidateIds, CancellationToken cancellationToken = default);

    [Obsolete]
    Task<List<DriveMember>> GetDriveMembersWithDetailsAsync(DriveMemberFilter filter, CancellationToken cancellationToken = default);

    Task<List<DriveMemberDTO>> GetDriveMembersAsDtoAsync(DriveMemberFilter filter, CancellationToken cancellationToken = default);

    [Obsolete]
    Task<List<DriveCandidate>> GetDriveCandidatesWithDetailsAsync(DriveCandidateFilter filter, CancellationToken cancellationToken = default);

    Task<List<DriveCandidateDTO>> GetDriveCandidatesAsDtoAsync(DriveCandidateFilter filter, CancellationToken cancellationToken = default);
    
    Task<List<Drive>> GetMentorDrivesWithCandidatesAsync(int mentorId,CancellationToken cancellationToken = default);

    Task<List<CandidateFeedbackDto>> GetCandidateFeedbackDetailsAsync(
        int candidateId,
        CancellationToken cancellationToken = default);

    Task<bool> IsDriveCandidateValidAsync(int driveId, int driveCandidateId);
    Task<List<CandidateDTO>> GetCandidatesWithoutRoundsAsync(
        int driveId,
        CancellationToken cancellationToken = default);
    #endregion

    #region DML

    void RemoveDriveMember(DriveMember driveMember);
    void RemoveDriveCandidate(DriveCandidate driveCandidate);

    #endregion
}