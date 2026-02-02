using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

public class DriveMember : BaseEntity
{
    public DriveMember() : base("drive_members")
    {
    }

    public int DriveMemberId { get; set; }
    public int DriveId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }

    // Navigation
    public Drive? Drive { get; set; }
    public User? User { get; set; }
    public Role? Role { get; set; }
    public ICollection<Round> InterviewedPanels { get; set; } = new List<Round>();
}
