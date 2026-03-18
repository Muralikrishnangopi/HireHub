using HireHub.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO
{
    public class PanelUserProjection
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string DriveName { get; set; } = string.Empty;
        public DateTime DriveDate { get; set; }

        public List<Availability> Availabilities { get; set; } = new();
    }
}