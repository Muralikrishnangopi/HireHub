using HireHub.Core.Data.Models;

namespace HireHub.Core.Data.Filters;

public class RoundFilter
{
    public int? DriveId { get; set; }
    public int? UserId { get; set; }
    public RoundType? RoundType { get; set; }
    public RoundStatus? RoundStatus { get; set; }
    public RoundResult? RoundResult { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
