using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO
{
    public class RequestMarkAttendances
    {
        public int driveId {  get; set; }

        public int CandidateId { get; set; }
        public  string attendanceStatus {  get; set; }
    }
}
