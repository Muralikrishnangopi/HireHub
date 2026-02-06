using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Shared.Persistence.Interface;

namespace HireHub.Core.Data.Interface;

public interface IRoundRepository : IGenericRepository<Round>
{
    #region DQL

    Task<int> CountInterviewsAsync(RoundStatus? roundStatus, CancellationToken cancellationToken = default);

    [Obsolete]
    Task<List<Round>> GetAllAsync(RoundFilter filter, CancellationToken cancellationToken = default);

    Task<List<RoundDTO>> GetAllAsDtoAsync(RoundFilter filter, CancellationToken cancellationToken = default);

    Task<RoundDTO?> GetByIdAsDtoAsync(int? roundId, CancellationToken cancellationToken = default);

    bool IsHrInterviewerForCandidateOnDrive(int userId, int candidateId, int driveId);

    bool IsInterviewerForRound(int userId, int roundId);

    Task<Round?> GetRoundByIdWithDetails(int roundId, CancellationToken cancellationToken = default);

    Task<List<Round>> GetRoundsForDriveCandidate(int driveCandidateId, CancellationToken cancellationToken = default);
    Task<List<DriveCandidate>> GetAllDriveCandidates(int driveId);

    Task<List<DriveMember>> GetAllDriveMember(int driveId);

   Task<Round?> GetOldInterviewer(int roundId, int oldInterviewerId);




    Task AssignNewInterview(Round round, int newInterviewerUserId);
    

    #endregion

    #region DML



    #endregion
}