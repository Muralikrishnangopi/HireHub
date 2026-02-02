using HireHub.Core.Data.Models;

namespace HireHub.Core.Data.Filters;

public class DriveFilter
{
    public DriveStatus? Status { get; set; }
    public string? CreatorEmail { get; set; }
    public int? TechnicalRounds { get; set; }
    public bool IsLatestFirst { get; set; } = true;
    public bool IncludePastDrives { get; set; } = false;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
