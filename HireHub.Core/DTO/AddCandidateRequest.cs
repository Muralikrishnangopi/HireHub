namespace HireHub.Core.DTO;

public class AddCandidateRequest
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Address { get; set; }
    public string? College { get; set; }
    public string? PreviousCompany { get; set; }
    public string ExperienceLevelName { get; set; } = null!;
    public List<string> TechStack { get; set; } = [];
    public string? ResumeUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
}
