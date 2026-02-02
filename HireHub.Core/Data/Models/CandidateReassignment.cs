using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

using System;

public class CandidateReassignment : BaseEntity
{
    public CandidateReassignment() : base("candidate_reassignments")
    {
    }

    public int ReassignId { get; set; }
    public int DriveCandidateId { get; set; }
    public int PreviousUserId { get; set; }
    public int NewUserId { get; set; }
    public int RequestedBy { get; set; }
    public bool RequireApproval { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime RequestedDate { get; set; } = DateTime.Now;

    // Navigation
    public DriveCandidate? DriveCandidate { get; set; }
    public DriveMember? PreviousUser { get; set; }
    public DriveMember? NewUser { get; set; }
    public User? Requester { get; set; }
    public User? Approver { get; set; }
}
