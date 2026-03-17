using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO
{
    public class MovetoNextRoundRequest
    {
        public int UserId {  get; set; }
        public int DriveCandidateId {  get; set; }
        public int RoundId {  get; set; }
        public int DriveMemberId { get; set; }

    }
}
