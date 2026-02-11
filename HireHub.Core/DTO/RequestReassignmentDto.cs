using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO
{
    public class RequestReassignmentDto
    {
        public int driveId { get; set; }
        public int candidateId { get; set; }
        public int previousUserId { get; set; }
        public int newUserId { get; set; }
        public bool requireApproval { get; set; }
    }
}