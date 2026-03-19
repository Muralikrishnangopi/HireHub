using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO
{
    public class CandidateStatusUpdateRequest
    {
        public int RoundId {  get; set; }
        public string? CandidateStatus {  get; set; }

    }
}
