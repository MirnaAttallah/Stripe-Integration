using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Stripe;
using Stripe.Checkout;
using Stripe_Integration.DTOs;
using Stripe_Integration.Models;
using Stripe_Integration.Repositories;
using System.Numerics;

namespace Stripe_Integration.Services
{
    public class PaymentService
    {
        private readonly InvoiceMainRepository invoiceRepository;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);
        

        private async Task<StripeClient> GetStripeClientAsync()
        {
            string kvUri = "https://doctorai-keyvault.vault.azure.net/";
            SecretClient client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            var secret = await client.GetSecretAsync("Stripe-DoctorAI-Key");
            return new StripeClient(apiKey: secret.Value.Value);
        }

        public PaymentService(InvoiceMainRepository invoiceRepository)
        {
            this.invoiceRepository = invoiceRepository;
        }

        public async Task<string> PaySubscription(CreateSubscriptionRequest request, ServiceMainDto plan)
        {
            var stripeClient = await GetStripeClientAsync();
            StripeConfiguration.ApiKey = stripeClient.ApiKey;

            var options = new SessionCreateOptions
            {
                UiMode = "embedded",
                LineItems = new List<SessionLineItemOptions>
                       {
                           new SessionLineItemOptions
                           {
                               PriceData = new SessionLineItemPriceDataOptions
                               {
                                   UnitAmountDecimal = request.Amount * 100m,
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
                TaxIdCollection = new SessionTaxIdCollectionOptions
                {
                    Enabled = true,
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
            return session.ClientSecret;
        }

        public async Task<string> PayOneTime(decimal price, string serviceName, string description, string userId)
        {
            var stripeClient = await GetStripeClientAsync();
            StripeConfiguration.ApiKey = stripeClient.ApiKey;

            var options = new SessionCreateOptions
            {
                SuccessUrl = "http://localhost:4200/success",
                Metadata = new Dictionary<string, string>()
                   {
                       { "B2CSubID", userId }
                   },
                LineItems = new List<SessionLineItemOptions>
                   {
                       new SessionLineItemOptions
                       {
                           PriceData = new SessionLineItemPriceDataOptions
                           {
                               UnitAmountDecimal = (long)(price * 100),
                               Currency = "usd",
                               ProductData = new SessionLineItemPriceDataProductDataOptions
                               {
                                   Name = serviceName,
                                   Description = description
                               },
                           },
                           Quantity = 1,
                       },
                   },
                Mode = "payment",
            };
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return session.Url;
        }

        public async Task SaveInvoice(InvoiceMain updatedInvoice)
        {
            await semaphore.WaitAsync();
            try
            {
                var existingInvoice = await invoiceRepository.GetById(updatedInvoice.InvoiceID);
                if (existingInvoice == null)
                {
                    throw new ArgumentException($"Couldn't find an invoice with ID: {updatedInvoice.InvoiceID}");
                }

                if (!string.IsNullOrEmpty(updatedInvoice.StripeInvoiceID))
                    existingInvoice.StripeInvoiceID = updatedInvoice.StripeInvoiceID;
                if (!string.IsNullOrEmpty(updatedInvoice.B2CSubID))
                    existingInvoice.B2CSubID = updatedInvoice.B2CSubID;

                if (!string.IsNullOrEmpty(updatedInvoice.StripeSubscriptionID))
                    existingInvoice.StripeSubscriptionID = updatedInvoice.StripeSubscriptionID;

                if (!string.IsNullOrEmpty(updatedInvoice.StripePaymentIntentID))
                    existingInvoice.StripePaymentIntentID = updatedInvoice.StripePaymentIntentID;

                if (!string.IsNullOrEmpty(updatedInvoice.StripeCustomerID))
                    existingInvoice.StripeCustomerID = updatedInvoice.StripeCustomerID;
                if (!string.IsNullOrEmpty(updatedInvoice.StripeCustomerEmail))
                    existingInvoice.StripeCustomerEmail = updatedInvoice.StripeCustomerEmail;

                if (!string.IsNullOrEmpty(updatedInvoice.StripeStatus))
                    existingInvoice.StripeStatus = updatedInvoice.StripeStatus;

                if (updatedInvoice.TotalTransactionAmount > 0)
                    existingInvoice.TotalTransactionAmount = updatedInvoice.TotalTransactionAmount;

                if (updatedInvoice.TaxAmount >= 0)
                    existingInvoice.TaxAmount = updatedInvoice.TaxAmount;

                if (updatedInvoice.PurchaseDate != null && updatedInvoice.PurchaseDate != DateTime.MinValue)
                    existingInvoice.PurchaseDate = updatedInvoice.PurchaseDate;

                await invoiceRepository.SaveChangesAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
