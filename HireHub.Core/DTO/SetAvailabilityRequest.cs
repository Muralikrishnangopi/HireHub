using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO
{
    public class SetAvailabilityRequest
    {
        public int userId {  get; set; }
        public List<DateTime> availabilityDates {  get; set; }
    }
}
