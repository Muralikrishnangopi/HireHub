namespace HireHub.Core.Data.Models;

using System;

public class Availability
{
    public int AvailabilityId { get; set; }
    public DateTime AvailabilityDate { get; set; }
    public int UserId { get; set; }

    // 🔗 Navigation property (optional but recommended)
    public User? User { get; set; }
}