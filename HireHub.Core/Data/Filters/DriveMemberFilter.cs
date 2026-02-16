using HireHub.Core.Data.Models;

namespace HireHub.Core.Data.Filters;

public class DriveMemberFilter
{
    public int? DriveId { get; set; }
    public List<int>? DriveIds {  get; set; }
    public int? UserId { get; set; }
    public UserRole? Role { get; set; }
    public DriveStatus? DriveStatus { get; set; }
    public List<DriveStatus>? DriveStatuses { get; set; }
    public bool? IsLatestFirst { get; set; } = true;
    public bool IncludePastDrives { get; set; } = false;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
