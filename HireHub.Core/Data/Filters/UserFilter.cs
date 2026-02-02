using HireHub.Core.Data.Models;

namespace HireHub.Core.Data.Filters;

public class UserFilter
{
    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public bool IsLatestFirst { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
