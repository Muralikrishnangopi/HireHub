namespace HireHub.Core.DTO;

public class AddMemberToDriveRequest
{
    public int DriveId { get; set; }
    public int MemberId { get; set; }
    public string MemberRole { get; set; } = null!;
}