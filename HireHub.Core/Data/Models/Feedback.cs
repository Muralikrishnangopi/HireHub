using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

using System;

public class Feedback : BaseEntity
{
    public Feedback() : base("feedbacks")
    {
    }

    public int FeedbackId { get; set; }
    public int? OverallRating { get; set; }
    public int? TechnicalSkill { get; set; }
    public int? Communication { get; set; }
    public int? ProblemSolving { get; set; }
    public string? OverallFeedback { get; set; }
    public Recommendation Recommendation { get; set; } = Recommendation.NA;
    public DateTime SubmittedDate { get; set; } = DateTime.Now;
}

public enum Recommendation
{
    NA,
    Hire,
    Maybe,
    NoHire
}

