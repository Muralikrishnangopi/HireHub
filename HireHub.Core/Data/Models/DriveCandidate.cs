using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

using System;
using System.Collections.Generic;

public class DriveCandidate : BaseEntity
{
    public DriveCandidate() : base("drive_candidates")
    {
    }

    public int DriveCandidateId { get; set; }
    public int CandidateId { get; set; }
    public int DriveId { get; set; }
    public CandidateStatus Status { get; set; } = CandidateStatus.Pending;
    public int? StatusSetBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Navigation
    public Candidate? Candidate { get; set; }
    public Drive? Drive { get; set; }
    public User? Recruiter { get; set; }
    public ICollection<Round> Rounds { get; set; } = new List<Round>();
    public ICollection<CandidateReassignment> CandidateReassignments { get; set; } = new List<CandidateReassignment>();
}

public enum CandidateStatus
{
    Pending,
    Selected,
    Rejected
}
