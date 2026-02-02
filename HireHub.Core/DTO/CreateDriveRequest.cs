namespace HireHub.Core.DTO;

public class CreateDriveRequest
{
    public string DriveName { get; set; } = null!;
    public DateTime DriveDate { get; set; }
    public int TechnicalRounds { get; set; }
    public CoordinationTeamRequest CoordinationTeam { get; set; } = null!;
    public HrConfigRequest HrConfiguration { get; set; } = null!;
    public MentorConfigRequest MentorConfiguration { get; set; } = null!;
    public PanelConfigRequest PanelConfiguration { get; set; } = null!;
    public PanelVisibilityConfigRequest PanelVisibilityConfiguration { get; set; } = null!;
    public NotificationConfigRequest NotificationConfiguration { get; set; } = null!;
    public FeedbackConfigRequest FeedbackSettings { get; set; } = null!;
}

public class CoordinationTeamRequest
{
    public List<int> Hrs { get; set; } = null!;
    public List<int> Mentors { get; set; } = null!;
    public List<int> PanelMembers { get; set; } = null!;
}

public class HrConfigRequest
{
    public bool AllowBulkUpload { get; set; }
    public bool CanEditSubmittedFeedback { get; set; }
    public bool AllowPanelReassign { get; set; } 
    public bool RequireApprovalForReassignment { get; set; }
}

public class PanelConfigRequest
{
    public bool CanEditSubmittedFeedback { get; set; }
    public bool AllowPanelReassign { get; set; }
    public bool RequireApprovalForReassignment { get; set; }
}

public class MentorConfigRequest
{
    public bool CanViewFeedback { get; set; }
    public bool AllowPanelReassign { get; set; }
    public bool RequireApprovalForReassignment { get; set; }
}

public class PanelVisibilityConfigRequest
{
    public bool ShowPhone { get; set; }
    public bool ShowEmail { get; set; }
    public bool ShowPreviousCompany { get; set; }
    public bool ShowResume { get; set; }
    public bool ShowCollege { get; set; }
    public bool ShowAddress { get; set; }
    public bool ShowLinkedIn { get; set; }
    public bool ShowGitHub { get; set; }
}

public class NotificationConfigRequest
{
    public bool EmailNotificationEnabled { get; set; }
}

public class FeedbackConfigRequest
{
    public bool OverallRatingRequired { get; set; }
    public bool TechnicalSkillRequired { get; set; }
    public bool CommunicationRequired { get; set; }
    public bool ProblemSolvingRequired { get; set; }
    public bool RecommendationRequired { get; set; }
    public bool OverallFeedbackRequired { get; set; }
}