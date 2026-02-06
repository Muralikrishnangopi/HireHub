using system;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO{
    public class ReassignPanel{
        public int roundId{get;set;}
        public int oldInterviewId{get;set;}
        public int newInterviewerId{get;set;}
    }
}