using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

using System;
using System.Collections.Generic;

public class Candidate : BaseEntity
{
    public Candidate() : base("candidates")
    {
    }

    public int CandidateId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Address { get; set; }
    public string? College { get; set; }
    public string? PreviousCompany { get; set; }
    public CandidateExperienceLevel ExperienceLevel { get; set; } = CandidateExperienceLevel.Fresher;
    public List<string> TechStack { get; set; } = [];
    public string? ResumeUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Navigation
    public ICollection<DriveCandidate> DriveCandidates { get; set; } = new List<DriveCandidate>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}

public enum CandidateExperienceLevel
{
    Fresher,
    Intermediate,
    Experienced
}
