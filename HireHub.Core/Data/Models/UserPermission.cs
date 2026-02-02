using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

public class UserPermission : BaseEntity
{
    public UserPermission() : base("user_permissions")
    {
    }

    // SQL didn't define a PK; we'll use composite key (UserId, Action) in EF mapping
    public int UserId { get; set; }
    public string Action { get; set; } = null!;
    public bool View { get; set; }
    public bool Add { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }

    // Navigation
    public User? User { get; set; }
}
