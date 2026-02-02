using HireHub.Core.Data.Models;

namespace HireHub.Core.Data.Filters;

public class DriveCandidateFilter
{
    public int? DriveId { get; set; }
    public int? CandidateId { get; set; }
    public CandidateStatus? CandidateStatus { get; set; }
    public DriveStatus? DriveStatus { get; set; }
    public bool IsLatestFirst { get; set; } = true;
    public bool IncludePastDrives { get; set; } = false;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
