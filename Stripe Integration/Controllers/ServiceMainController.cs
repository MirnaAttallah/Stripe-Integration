using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe_Integration.Models;
using Stripe_Integration.Services;
using System.Numerics;
using System.Text;

namespace Stripe_Integration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceMainController : ControllerBase
    {
        private readonly ServiceMainService serviceMainService;
        private readonly InvoiceMainService invoiceMainService; 
        private readonly PaymentService paymentService;
        public ServiceMainController(ServiceMainService _serviceMainService, PaymentService _paymentService,
            InvoiceMainService _invoiceMainService
            )
        {
            serviceMainService = _serviceMainService;
            invoiceMainService = _invoiceMainService;
            paymentService = _paymentService;
        }

        [HttpGet("/subscribe")]
        public async Task<string> SubscribeToPlan(int planId, string userId, int invoiceId)
        {
            var plan = await serviceMainService.GetServiceById(planId);
            var features = new StringBuilder();
            plan.ServiceDetails.ToList().ForEach(d => features.AppendLine( $"{d.MonthlyCount ?? 'A'} {d.DetailItemDescription}"));
            var checkoutUrl = await paymentService.PaySubscription(plan.Price, plan.ShortDescription, features.ToString(), userId, invoiceId);
            //await invoiceMainService.SwitchInvoiceStatus(userId, "pending");
            await invoiceMainService.SwitchInvoiceStatus(invoiceId, "pending");

            return checkoutUrl;
        }
        [HttpGet("/plans")]
        public async Task<List<ServiceMainDto>> GetPlans()
        {
            var plans = await serviceMainService.GetPlans();
            return plans;
        }
        [HttpGet("/available-labs")]
        public async Task<List<ServiceMainDto>> GetAvailableLabs()
        {
            var labTests = await serviceMainService.GetLabTests();
            return labTests;
        }

        [HttpGet("/request/labtest")]
        public async Task<string> RequestLabTest(int serviceId, string userId)
        {
            var service = await serviceMainService.GetServiceById(serviceId);
            var features = service.ServiceDetails.FirstOrDefault()!.DetailItemDescription;
            var checkoutUrl = await paymentService.PayOneTime(service.Price, service.ShortDescription, features, userId);
            return checkoutUrl;
        }
    }
}
