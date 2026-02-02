using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using Microsoft.Extensions.Logging;

namespace HireHub.Core.Service;

public class AdminService
{
    private readonly IUserRepository _userRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IRoundRepository _roundRepository;
    private readonly ISaveRepository _saveRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IUserRepository userRepository,
        ICandidateRepository candidateRepository,IRoundRepository roundRepository,
        ISaveRepository saveRepository, ILogger<AdminService> logger)
    {
        _userRepository = userRepository;
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
            TotalUsers = await _userRepository.CountUsersAsync(),
            TotalCandidates = await _candidateRepository.CountCandidatesAsync(),
            TotalPanelMembers = await _userRepository.CountUsersByRoleAsync(UserRole.Panel),
            TotalMentors = await _userRepository.CountUsersByRoleAsync(UserRole.Mentor),
            TotalHrs = await _userRepository.CountUsersByRoleAsync(UserRole.HR),
            TotalInterviews = await _roundRepository.CountInterviewsAsync(null),
            InterviewsScheduled = await _roundRepository.CountInterviewsAsync(RoundStatus.Scheduled),
            InterviewsOnProcess = await _roundRepository.CountInterviewsAsync(RoundStatus.OnProcess),
            InterviewsCompleted = await _roundRepository.CountInterviewsAsync(RoundStatus.Completed),
            InterviewsSkipped = await _roundRepository.CountInterviewsAsync(RoundStatus.Skipped),
            TotalCandidatesHired = await _candidateRepository.CountByDriveStatusAsync(CandidateStatus.Selected),
            TotalCandidatesRejected = await _candidateRepository.CountByDriveStatusAsync(CandidateStatus.Rejected)
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
