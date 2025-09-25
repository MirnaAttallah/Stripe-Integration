using Microsoft.EntityFrameworkCore;
using Stripe_Integration.Context;
using Stripe_Integration.Models;

namespace Stripe_Integration.Repositories
{
    public class InvoiceMainRepository : Repository<InvoiceMain>
    {
        private readonly StripeDbContext _context;
        public InvoiceMainRepository(StripeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceMain>> GetAllByCustomerId(string userId)
        {
            return await _context.InvoiceMain.Where(i => i.B2CSubID == userId).ToListAsync();
        }
        public async Task<List<InvoiceMain>> GetAllByCustomerAndDuration(string customerId, TimeSpan duration)
        {
            var invoices = await _context.InvoiceMain.Where(i => i.StripeCustomerID == customerId || i.StripeCustomerEmail == customerId)
                .Where(i => i.PurchaseDate >= DateTime.UtcNow.Subtract(duration))
                .OrderByDescending(i => i.PurchaseDate)
                .ToListAsync();
            return invoices;
        }

        public async Task<List<InvoiceDetail>> GetAllDetailsByInvoiceId(int invoiceId)
        {
            return await _context.InvoiceDetails
                                 .Where(detail => detail.InvoiceID == invoiceId)
                                 .ToListAsync();
        }

    }
}
