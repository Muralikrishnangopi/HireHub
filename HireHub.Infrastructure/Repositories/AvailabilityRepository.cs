using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Infrastructure.Repositories
{
    public class AvailabilityRepository:GenericRepository<Availability>,IAvailbilityRepository
    {
        private new readonly HireHubDbContext _context;
        public AvailabilityRepository(HireHubDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
