using Microsoft.EntityFrameworkCore;
using Stripe_Integration.Context;
using Stripe_Integration.Models;

namespace Stripe_Integration.Repositories
{
    public class ServiceMainRepository : Repository<ServiceMain>
    {
        private readonly StripeDbContext _context;
        public ServiceMainRepository(StripeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ServiceMain>> GetAllByType(string type)
        {
            return await _context.ServiceMain.Where(s=> s.ServiceType == type).ToListAsync();
        }
    }
}
