namespace HireHub.Core.DTO;

public class AddFeedbackRequest
{
    public int RoundId { get; set; }
    public int? OverallRating { get; set; }
    public string? TechnicalSkill { get; set; }
    public string? Communication { get; set; }
    public string? ProblemSolving { get; set; }
    public string? OverallFeedback { get; set; }
    public string CandidateRecommendation { get; set; } = null!;
}
