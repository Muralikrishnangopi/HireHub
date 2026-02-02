using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

public class PanelVisibilitySettings : BaseEntity
{
    public PanelVisibilitySettings() : base("panel_visibility_settings")
    {
    }

    public int VisibilityId { get; set; }
    public int DriveId { get; set; }
    public bool ShowPhone { get; set; } = true;
    public bool ShowEmail { get; set; } = true;
    public bool ShowPreviousCompany { get; set; } = true;
    public bool ShowResume { get; set; } = true;
    public bool ShowCollege { get; set; } = true;
    public bool ShowAddress { get; set; } = true;
    public bool ShowLinkedIn { get; set; } = true;
    public bool ShowGitHub { get; set; } = true;

    // Navigation
    public Drive? Drive { get; set; }
}
