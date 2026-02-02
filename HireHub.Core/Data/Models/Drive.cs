using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

using System;
using System.Collections.Generic;

public class Drive : BaseEntity
{
    public Drive() : base("drives")
    {
    }

    public int DriveId { get; set; }
    public string DriveName { get; set; } = null!;
    public DateTime DriveDate { get; set; }
    public int TechnicalRounds { get; set; }
    public DriveStatus Status { get; set; } = DriveStatus.InProposal;
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Navigation
    public User? Creator { get; set; }
    public ICollection<DriveMember> DriveMembers { get; set; } = new List<DriveMember>();
    public ICollection<DriveRoleConfiguration> DriveRoleConfigurations { get; set; } = new List<DriveRoleConfiguration>();
    public PanelVisibilitySettings? PanelVisibilitySettings { get; set; }
    public NotificationSettings? NotificationSettings { get; set; }
    public FeedbackConfiguration? FeedbackConfiguration { get; set; }
    public ICollection<DriveCandidate> DriveCandidates { get; set; } = new List<DriveCandidate>();
    public ICollection<Round> Rounds { get; set; } = new List<Round>();
    public ICollection<Request> Requests { get; set; } = new List<Request>();
}

public enum DriveStatus
{
    InProposal,
    Started,
    Halted,
    Completed,
    Cancelled
}
