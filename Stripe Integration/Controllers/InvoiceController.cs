using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe_Integration.DTOs;
using Stripe_Integration.Services;
using Stripe_Integration.Models;

namespace Stripe_Integration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceMainService _invoiceMainService;
        public InvoiceController(InvoiceMainService invoiceMainService)
        {
            _invoiceMainService = invoiceMainService;
        }
        [HttpGet("{id}")]
        public async Task<CreateSubscriptionRequest> GetInvoice(int id)
        {
            var invoice = await _invoiceMainService.GetInvoiceById(id);
            CreateSubscriptionRequest subscriptionRequest = new CreateSubscriptionRequest() { InvoiceId = id, B2cSubId = invoice.B2CSubID };
            var invoiceDetails = invoice.InvoiceDetails;
            invoiceDetails.ToList().ForEach(i => subscriptionRequest.Amount += i.Amount);
            subscriptionRequest.PlanId = invoiceDetails.FirstOrDefault()!.ServiceMainID;
            subscriptionRequest.Interval = invoiceDetails.FirstOrDefault()!.Frequency;
            return subscriptionRequest;
        }

        [HttpPost("/add-to-cart/{cartId}")]
        public async Task AddToCart([FromBody] CartItem cartItem, int cartId)
        {
            var existingInvoice = await _invoiceMainService.GetInvoiceById(cartId);
            if (existingInvoice.InvoiceDetails.Any(d => d.ServiceMainID == cartItem.ServiceMainId))
            {
                var existingDetail = existingInvoice.InvoiceDetails.First(d => d.ServiceMainID == cartItem.ServiceMainId);
                existingDetail.Quantity += cartItem.Quantity;
                await _invoiceMainService.UpdateInvoiceDetailAsync(existingDetail);
            }
            else
            {
                var invoiceDetail = new InvoiceDetail
                {
                    Amount = cartItem.UnitAmount,
                    InvoiceID = cartId,
                    ServiceMainID = cartItem.ServiceMainId,
                    Frequency = "one-time",
                    Quantity = cartItem.Quantity
                };
                await _invoiceMainService.AddInvoiceDetail(invoiceDetail);
            }
        }
    }
}
