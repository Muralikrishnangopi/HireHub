using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO
{
    public class ManualAssignmentCandidate
    {
        public int driveId { get; set; }
        public int driveCandidateId { get; set; }

        public int userId { get; set; }
    }
}
