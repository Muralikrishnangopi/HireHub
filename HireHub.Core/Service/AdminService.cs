using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using Microsoft.Extensions.Logging;

namespace HireHub.Core.Service;

public class AdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IDriveRepository _driveRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IRoundRepository _roundRepository;
    private readonly ISaveRepository _saveRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IUserRepository userRepository, IDriveRepository driveRepository,
        ICandidateRepository candidateRepository,IRoundRepository roundRepository,
        ISaveRepository saveRepository, ILogger<AdminService> logger)
    {
        _userRepository = userRepository;
        _driveRepository = driveRepository;
        _candidateRepository = candidateRepository;
        _roundRepository = roundRepository;
        _saveRepository = saveRepository;
        _logger = logger;
    }


    #region Query Services

    public async Task<Response<AdminDashboardDetails>> GetDashboardDetails()
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDashboardDetails));

        var dashboard = new AdminDashboardDetails
        {
            TotalUsers = await _userRepository.CountUsersAsync(null),
            ActiveUsers = await _userRepository.CountUsersAsync(true),
            InactiveUsers = await _userRepository.CountUsersAsync(false),
            TotalPanelMembers = await _userRepository.CountUsersByRoleAsync(UserRole.Panel),
            TotalMentors = await _userRepository.CountUsersByRoleAsync(UserRole.Mentor),
            TotalHrs = await _userRepository.CountUsersByRoleAsync(UserRole.HR),
            TotalCandidates = await _candidateRepository.CountCandidatesAsync(),
            TotalCandidatesHired = await _candidateRepository.CountByDriveStatusAsync(CandidateStatus.Selected),
            TotalCandidatesRejected = await _candidateRepository.CountByDriveStatusAsync(CandidateStatus.Rejected),
            TotalDrives = await _driveRepository.CountDrivesAsync(null),
            ScheduledDrives = await _driveRepository.CountDrivesAsync(DriveStatus.InProposal),
            OngoingDrives = await _driveRepository.CountDrivesAsync(DriveStatus.Started),
            HaltedDrives = await _driveRepository.CountDrivesAsync(DriveStatus.Halted),
            CompletedDrives = await _driveRepository.CountDrivesAsync(DriveStatus.Completed),
            CancelledDrives = await _driveRepository.CountDrivesAsync(DriveStatus.Cancelled),
            TotalInterviews = await _roundRepository.CountInterviewsAsync(null),
            InterviewsScheduled = await _roundRepository.CountInterviewsAsync(RoundStatus.Scheduled),
            InterviewsOnProcess = await _roundRepository.CountInterviewsAsync(RoundStatus.OnProcess),
            InterviewsCompleted = await _roundRepository.CountInterviewsAsync(RoundStatus.Completed),
            InterviewsSkipped = await _roundRepository.CountInterviewsAsync(RoundStatus.Skipped)
        };

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetDashboardDetails));

        return new()
        {
            Data = dashboard
        };
    }

    #endregion

    #region Command Services



    #endregion

    #region Private Methods



    #endregion
}
