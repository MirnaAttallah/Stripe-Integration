using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe_Integration.DTOs;
using Stripe_Integration.Models;
using Stripe_Integration.Services;
using System.Numerics;

namespace Stripe_Integration.Controllers
{
    // PaymentController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly ServiceMainService _serviceMainService;
        private readonly InvoiceMainService _invoiceMainService;

        public PaymentController(PaymentService paymentService, ServiceMainService serviceMainService,
            InvoiceMainService invoiceMainService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _serviceMainService = serviceMainService;
            _invoiceMainService = invoiceMainService;
            _logger = logger;
        }

        [HttpPost("/subscription-invoice")]
        public async Task<int> CreateSubscriptionInvoice([FromBody] CreateSubscriptionInvoice request)
        {
            var invoice = new InvoiceMain
            {
                B2CSubID = request.B2cSubId,
                StripeStatus = "inprogress",
                TotalTransactionAmount = request.Amount
            };
            var createdInvoice = await _invoiceMainService.CreateInvoiceAsync(invoice);
            var InvoiceDetail = new InvoiceDetail
            {
                Amount = request.Amount,
                InvoiceID = createdInvoice.InvoiceID,
                ServiceMainID = request.PlanId,
                Frequency = request.Interval,
                Quantity = 1
            };
            await _invoiceMainService.AddInvoiceDetail(InvoiceDetail);
            return createdInvoice.InvoiceID;
        }

        [HttpPost("/payment-list-invoice")]
        public async Task<int> CreatePaymentInvoice([FromBody] CreatePaymentInvoice request)
        {
            var createdInvoiceId = await _invoiceMainService.CreateInvoiceAsync(request);
            return createdInvoiceId;
        }

        [HttpGet("/invoiceType/{id}")]
        public async Task<bool> IsShoppingCart(int id)
        {
            var invoice = await _invoiceMainService.GetInvoiceById(id);
            if(invoice.InvoiceDetails.Any(d => d.ServiceMain.ServiceType == "Lab Work"))
            {
                return true;
            }
            return false;
        }

        [HttpPost("create-subscription")]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
        {
            try
            {
                var plan = await _serviceMainService.GetPlanById(request.PlanId);
                var options = new SessionCreateOptions
                {
                    UiMode = "embedded",
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmountDecimal = request.Amount *100m,
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = plan.Name + " Plan",
                                    Description = plan.Features != null ? string.Join(", ", plan.Features) : "",
                                },
                                Currency = request.Currency,
                                Recurring = new SessionLineItemPriceDataRecurringOptions
                                {
                                    Interval = request.Interval
                                }
                            },
                            Quantity = 1,
                        },
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "invoiceId", request.InvoiceId.ToString() },
                    },
                    Mode = "subscription",
                    ReturnUrl = "http://localhost:4200/return?session_id={CHECKOUT_SESSION_ID}",
                    AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
                };
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                //await _invoiceMainService.SwitchInvoiceStatus(request.InvoiceId, "inprogress");
                //await _invoiceMainService.AddInvoiceDetail(new InvoiceDetail
                //{
                //    Amount = request.Amount,
                //    InvoiceID = request.InvoiceId,
                //    ServiceMainID = request.PlanId,
                //    Frequency = request.Interval,
                //    Quantity = 1
                //});
                return Ok(new { clientSecret = session.ClientSecret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription payment intent");
                return BadRequest(new { error = "Failed to create payment intent" });
            }
        }

        [HttpGet("/create-payment/{invoiceId}")]
        public async Task<IActionResult> CreatePayment(int invoiceId)
        {
            try
            {
                var invoiceDetails = await _invoiceMainService.GetInvoiceDetailsById(invoiceId);
                var options = new SessionCreateOptions
                {
                    UiMode = "embedded",
                    LineItems = new List<SessionLineItemOptions>
                    {

                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "invoiceId", invoiceId.ToString() },
                    },
                    Mode = "payment",
                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        Metadata = new Dictionary<string, string>
                        {
                            { "invoiceId", invoiceId.ToString() },
                        }
                    },
                    ReturnUrl = "http://localhost:4200/return?session_id={CHECKOUT_SESSION_ID}",
                    AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
                    InvoiceCreation = new SessionInvoiceCreationOptions
                    {
                        Enabled = true
                    }
                };
                invoiceDetails.ForEach(detail =>
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmountDecimal = detail.Amount * 100m,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = detail.ServiceMain.ShortDescription,
                            },
                            Currency = "usd",
                        },
                        Quantity = detail.Quantity,
                    });
                });
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return Ok(new { clientSecret = session.ClientSecret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription payment intent");
                return BadRequest(new { error = "Failed to create payment intent" });
            }
        }

        [HttpGet("/cart/{id}")]
        public async Task<CartDTO> GetCart(int id)
        {
            var invoice = await _invoiceMainService.GetInvoiceById(id);
            var invoiceDetails = await _invoiceMainService.GetInvoiceDetailsById(id);
            var cart = new CartDTO()
            {
                B2cSubId = invoice.B2CSubID,
                CreatedIn = invoice.PurchaseDate ?? DateTime.UtcNow,
                Currency = "usd",
                Items = new List<CartItem>()
            };
            invoiceDetails.ForEach(i => cart.Items.Add(new CartItem
            {
                ServiceMainId = i.ServiceMainID,
                UnitAmount = i.Amount,
                Quantity = i.Quantity,
            }));
            return cart;
        }

        [HttpGet("/session-status")]
        public async Task<ActionResult> SessionStatus([FromQuery] string session_id)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);
            var invoiceId = int.Parse(session.Metadata["invoiceId"]);
            var invoice = await _invoiceMainService.GetInvoiceById(invoiceId);
            invoice.StripeInvoiceID = session.InvoiceId;
            invoice.StripeCustomerID = session.CustomerId;
            invoice.StripeSubscriptionID = session.SubscriptionId;
            invoice.StripeCustomerEmail = session.CustomerEmail ?? session.CustomerDetails.Email;
            invoice.StripeStatus = session.PaymentStatus;
            invoice.TaxAmount = session.TotalDetails.AmountTax;
            invoice.TotalTransactionAmount = (decimal)session.AmountTotal!/100m; //after discounts and taxes
            invoice.PurchaseDate = session.Created;
            if (session.Mode == "payment")
                invoice.StripePaymentIntentID = session.PaymentIntentId;
            await _invoiceMainService.UpdateAsync(invoice);
            return new JsonResult(new { status = session.Status, customer_email = session.CustomerDetails.Email, invoiceId = invoiceId });
        }
    }
}
