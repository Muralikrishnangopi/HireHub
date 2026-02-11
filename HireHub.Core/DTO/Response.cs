using HireHub.Shared.Common.Models;

namespace HireHub.Core.DTO;

public class Response<T> : BaseResponse where T : class
{
    public T? Data { get; set; } = null;
}

public class AdminDashboardDetails
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int TotalPanelMembers { get; set; }
    public int TotalMentors { get; set; }
    public int TotalHrs { get; set; }
    public int TotalCandidates { get; set; }
    public int TotalCandidatesHired { get; set; }
    public int TotalCandidatesRejected { get; set; }
    public int TotalDrives { get; set; }
    public int ScheduledDrives { get; set; }
    public int OngoingDrives { get; set; }
    public int HaltedDrives { get; set; }
    public int CompletedDrives { get; set; }
    public int CancelledDrives { get; set; }
    public int TotalInterviews { get; set; }
    public int InterviewsScheduled { get; set; }
    public int InterviewsOnProcess { get; set; }
    public int InterviewsCompleted { get; set; }
    public int InterviewsSkipped { get; set; }
}

public class UserDTO
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class CandidateDTO
{
    public int CandidateId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Address { get; set; }
    public string? College { get; set; }
    public string? PreviousCompany { get; set; }
    public string CandidateExperienceLevel { get; set; } = null!;
    public List<string> TechStack { get; set; } = [];
    public string? ResumeUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class DriveDTO
{
    public int DriveId { get; set; }
    public string DriveName { get; set; } = null!;
    public DateTime DriveDate { get; set; }
    public int TechnicalRounds { get; set; }
    public string DriveStatus { get; set; } = null!;
    public int CreatedBy { get; set; }
    public string CreatorName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
}

public class HrConfigurationDTO
{
    public int ConfigId { get; set; }
    public int DriveId { get; set; }
    public bool AllowBulkUpload { get; set; }
    public bool CanEditSubmittedFeedback { get; set; }
    public bool AllowPanelReassign { get; set; }
    public bool RequireApprovalForReassignment { get; set; }
}
public class PanelConfigurationDTO
{
    public int ConfigId { get; set; }
    public int DriveId { get; set; }
    public bool CanEditSubmittedFeedback { get; set; }
    public bool AllowPanelReassign { get; set; }
    public bool RequireApprovalForReassignment { get; set; }
}
public class MentorConfigurationDTO
{
    public int ConfigId { get; set; }
    public int DriveId { get; set; }
    public bool CanViewFeedback { get; set; }
    public bool AllowPanelReassign { get; set; }
    public bool RequireApprovalForReassignment { get; set; }
}

public class PanelVisibilitySettingsDTO
{
    public int VisibilityId { get; set; }
    public int DriveId { get; set; }
    public bool ShowPhone { get; set; }
    public bool ShowEmail { get; set; }
    public bool ShowPreviousCompany { get; set; }
    public bool ShowResume { get; set; }
    public bool ShowCollege { get; set; }
    public bool ShowAddress { get; set; }
    public bool ShowLinkedIn { get; set; }
    public bool ShowGitHub { get; set; }
}

public class NotificationSettingsDTO
{
    public int NotificationId { get; set; }
    public int DriveId { get; set; }
    public bool EmailNotificationEnabled { get; set; }
}

public class FeedbackConfigurationDTO
{
    public int FeedbackConfigId { get; set; }
    public int DriveId { get; set; }
    public bool OverallRatingRequired { get; set; }
    public bool TechnicalSkillRequired { get; set; }
    public bool CommunicationRequired { get; set; }
    public bool ProblemSolvingRequired { get; set; }
    public bool RecommendationRequired { get; set; }
    public bool OverallFeedbackRequired { get; set; }
}

public class DriveConfigDTO
{
    public int DriveId { get; set; }
    public HrConfigurationDTO HrConfiguration { get; set; } = null!;
    public PanelConfigurationDTO PanelConfiguration { get; set; } = null!;
    public MentorConfigurationDTO MentorConfiguration { get; set; } = null!;
    public PanelVisibilitySettingsDTO PanelVisibilitySettings { get; set; } = null!;
    public NotificationSettingsDTO NotificationSettings { get; set; } = null!;
    public FeedbackConfigurationDTO FeedbackConfiguration { get; set; } = null!;
}

public class DriveCandidateDTO
{
    public int DriveCandidateId { get; set; }
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = null!;
    public string CandidateEmail { get; set; } = null!;
    public int DriveId { get; set; }
    public string DriveName { get; set; } = null!;
    public DateTime DriveDate { get; set; }
    public string DriveStatus { get; set; } = null!;
    public string CandidateStatus { get; set; } = null!;
    public int? StatusSetBy { get; set; }
}

public class DriveMemberDTO
{
    public int DriveMemberId { get; set; }
    public int DriveId { get; set; }
    public string DriveName { get; set; } = null!;
    public DateTime DriveDate { get; set; }
    public string DriveStatus { get; set; } = null!;
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
}

public class RoundDTO
{
    public int RoundId { get; set; }
    public int DriveId { get; set; }
    public string DriveName { get; set; } = null!;
    public DateTime DriveDate { get; set; }
    public string DriveStatus { get; set; } = null!;
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = null!;
    public string CandidateEmail { get; set; } = null!;
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string RoundStatus { get; set; } = null!;
    public string RoundResult { get; set; } = null!;
    public int? FeedbackId { get; set; } = null;
}

public class FeedbackDTO
{
    public int FeedbackId { get; set; }
    public int? OverallRating { get; set; }
    public int? TechnicalSkill { get; set; }
    public int? Communication { get; set; }
    public int? ProblemSolving { get; set; }
    public string? OverallFeedback { get; set; }
    public string CandidateRecommendation { get; set; } = null!;
    public DateTime SubmittedDate { get; set; }
}

public class DriveWithCandidatesDto
{
    public int DriveId { get; set;}
    public string? DriveName { get; set; }
	public DateTime DriveDate { get; set; }
    public List<MentorCandidateDto>? Candidates { get; set; } = new();
}

public class AutoAssignDTO{
    public int? DriveId{get;set;}
}

public class AvailabilityDTO
{
    public int AvailabilityId {  get; set; }
    public DateTime AvailabilityDate { get; set; }
    public int UserId { get; set; }
}