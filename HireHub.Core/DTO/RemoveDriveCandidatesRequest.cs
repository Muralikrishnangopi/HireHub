namespace HireHub.Core.DTO;

public class RemoveDriveCandidatesRequest
{
    public int DriveId { get; set; }
    public List<int> CandidateIds { get; set; } = [];
}