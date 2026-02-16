using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Shared.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Infrastructure.Repositories
{
    public class AvailabilityRepository : GenericRepository<Availability>, IAvailbilityRepository
    {
        private new readonly HireHubDbContext _context;
        public AvailabilityRepository(HireHubDbContext context):base(context) 
        {
            _context = context;
        }

        public async Task<List<User>> GetUserForDriveAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.Availabilities
                        .Where(a => a.AvailabilityDate.Date == date.Date) // compare dates only
                        .Include(a => a.User)                              // include the User entity
                        .Select(a => a.User!)                              // select the User from Availability
                        .ToListAsync(cancellationToken);
        }

        public async Task<List<Availability>> GetAvailabiltyBasedonUserId(List<int> UserIds,CancellationToken cancellationToken=default)
        {
            var availabilities = await _context.Availabilities
                                .Where(a => UserIds.Contains(a.UserId))
                                .ToListAsync();
            return availabilities;
        }
    }
}
