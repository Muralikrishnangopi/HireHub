using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

using System.Collections.Generic;

public class Role : BaseEntity
{
    public Role() : base("roles")
    {
    }

    public int RoleId { get; set; }
    public UserRole RoleName { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<DriveRoleConfiguration> DriveRoleConfigurations { get; set; } = new List<DriveRoleConfiguration>();
    public ICollection<DriveMember> DriveMembers { get; set; } = new List<DriveMember>();
}

public enum UserRole
{
    Admin,
    HR,
    Mentor,
    Panel
}