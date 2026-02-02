namespace HireHub.Core.DTO;

public class AddFeedbackRequest
{
    public int RoundId { get; set; }
    public int? OverallRating { get; set; }
    public int? TechnicalSkill { get; set; }
    public int? Communication { get; set; }
    public int? ProblemSolving { get; set; }
    public string? OverallFeedback { get; set; }
    public string CandidateRecommendation { get; set; } = null!;
}
