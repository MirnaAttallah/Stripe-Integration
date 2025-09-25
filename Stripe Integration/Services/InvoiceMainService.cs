using Stripe_Integration.DTOs;
using Stripe_Integration.Models;
using Stripe_Integration.Repositories;

namespace Stripe_Integration.Services
{
    public class InvoiceMainService
    {
        private readonly InvoiceMainRepository _invoiceRepository;
        private readonly Repository<InvoiceDetail> _invoiceDetailRepository;
        public InvoiceMainService(InvoiceMainRepository invoiceRepository, Repository<InvoiceDetail> invoiceDetailRepository)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
        }

        public async Task<int> CreateInvoiceAsync(CreatePaymentInvoice request)
        {

            var invoice = new InvoiceMain
            {
                B2CSubID = request.B2cSubId,
                StripeStatus = "inprogress",
                TotalTransactionAmount = 0m,
            };
            request.Items.ForEach(i =>
            {
                invoice.TotalTransactionAmount += (i.UnitAmount * i.Quantity);
            });
            var createdInvoice = await _invoiceRepository.AddAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();
            var InvoiceDetail = new InvoiceDetail
            {
                Amount = request.Items[0].UnitAmount,
                InvoiceID = createdInvoice.InvoiceID,
                ServiceMainID = request.Items[0].ServiceMainId,
                Frequency = "one-time",
                Quantity = request.Items[0].Quantity
            };
            await AddInvoiceDetail(InvoiceDetail);
            return createdInvoice.InvoiceID;
        }
        public async Task<InvoiceMain> CreateInvoiceAsync(InvoiceMain invoice)
        {
            var createdInvoice = await _invoiceRepository.AddAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();
            return createdInvoice;
        }

        public async Task<InvoiceMain> GetInvoiceById(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetById(invoiceId);
            return invoice;
        }
        public async Task<List<InvoiceDetail>> GetInvoiceDetailsById(int invoiceId)
        {
            var invoiceDetails = await _invoiceRepository.GetAllDetailsByInvoiceId(invoiceId);
            return invoiceDetails;
        }
        public async Task AddInvoiceDetail(InvoiceDetail invoiceDetail)
        {
            await _invoiceDetailRepository.AddAsync(invoiceDetail);
            await _invoiceDetailRepository.SaveChangesAsync();
        }

        public async Task UpdateInvoiceDetailAsync(InvoiceDetail invoiceDetail)
        {
            _invoiceDetailRepository.UpdateAsync(invoiceDetail);
            await _invoiceDetailRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(InvoiceMain invoice)
        {
            _invoiceRepository.UpdateAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();
        }

        public async Task SwitchInvoiceStatus(int invoiceId, string status)
        {
            InvoiceMain invoice = await _invoiceRepository.GetById(invoiceId);
            invoice.StripeStatus = status;
            _invoiceRepository.UpdateAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();
        }

        public async Task<List<InvoiceMain>> GetLatestInvoicesByCustomer(string customer, TimeSpan duration)
        {
            var invoices = await _invoiceRepository.GetAllByCustomerAndDuration(customer, duration);
            return invoices;
        }
    }
}
