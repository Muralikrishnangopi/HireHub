namespace HireHub.Core.DTO;

public class AddCandidatesToDriveRequest
{
    public int DriveId { get; set; }
    public List<int> CandidateIds { get; set; } = [];
}