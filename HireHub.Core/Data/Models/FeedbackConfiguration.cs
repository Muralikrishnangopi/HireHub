using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

public class FeedbackConfiguration : BaseEntity
{
    public FeedbackConfiguration() : base("feedback_configuration")
    {
    }

    public int FeedbackConfigId { get; set; }
    public int DriveId { get; set; }
    public bool OverallRatingRequired { get; set; } = true;
    public bool TechnicalSkillRequired { get; set; } = true;
    public bool CommunicationRequired { get; set; } = true;
    public bool ProblemSolvingRequired { get; set; } = true;
    public bool RecommendationRequired { get; set; } = true;
    public bool OverallFeedbackRequired { get; set; } = true;

    // Navigation
    public Drive? Drive { get; set; }
}
