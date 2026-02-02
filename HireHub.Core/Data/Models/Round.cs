using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

public class Round : BaseEntity
{
    public Round() : base("rounds")
    {
    }

    public int RoundId { get; set; }
    public int DriveCandidateId { get; set; }
    public int InterviewerId { get; set; }
    public RoundType RoundType { get; set; } = RoundType.Tech1;
    public RoundStatus Status { get; set; } = RoundStatus.Scheduled;
    public RoundResult Result { get; set; } = RoundResult.Pending;
    public int? FeedbackId { get; set; } = null;

    // Navigation
    public DriveCandidate? DriveCandidate { get; set; }
    public DriveMember? Interviewer { get; set; }
    public Feedback? Feedback { get; set; }
}

public enum RoundType
{
    Hr,
    Tech1,
    Tech2
}

public enum RoundStatus
{
    Scheduled,
    OnProcess,
    Completed,
    Skipped
}

public enum RoundResult
{
    Pending,
    Selected,
    Rejected
}
