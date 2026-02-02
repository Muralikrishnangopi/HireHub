using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

public class NotificationSettings : BaseEntity
{
    public NotificationSettings() : base("notification_settings")
    {
    }

    public int NotificationId { get; set; }
    public int DriveId { get; set; }
    public bool EmailNotificationEnabled { get; set; } = true;

    // Navigation
    public Drive? Drive { get; set; }
}
