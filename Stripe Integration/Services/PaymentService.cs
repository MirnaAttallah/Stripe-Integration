using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Stripe;
using Stripe.Checkout;
using Stripe_Integration.Models;
using Stripe_Integration.Repositories;
using static System.Net.WebRequestMethods;

namespace Stripe_Integration.Services
{
    public class PaymentService
    {
        private readonly InvoiceMainRepository invoiceRepository;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);
        public PaymentService(InvoiceMainRepository invoiceRepository)
        {
            //string keyVaultName = Environment.GetEnvironmentVariable("Meliora-Key-Vault");

            var kvUri = "https://meliora-key-vault.vault.azure.net/";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            var secret = client.GetSecret("Stripe-DoctorAI-Key");
            StripeConfiguration.ApiKey = secret.Value.Value;
            this.invoiceRepository = invoiceRepository;
        }

        public async Task<string> PaySubscription(decimal price, string productName, string description, string userId, int invoiceId)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = "http://localhost:4200",
                AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
                Metadata = new Dictionary<string, string>(){
                    { "B2CSubID", userId },

                    {"invoiceID", invoiceId.ToString() }
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmountDecimal = (long)(price * 100),
                            Currency = "usd",
                            Recurring = new SessionLineItemPriceDataRecurringOptions
                            {
                                Interval = "month",
                            },
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = productName,
                                Description = description
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "subscription",

                //TaxIdCollection = new SessionTaxIdCollectionOptions
                //{
                //    Enabled = true,
                //},
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            await invoiceRepository.AddAsync(new InvoiceMain
            {
                B2CSubID = userId,
                TotalTransactionAmount = price,
                StripeStatus = "inprogress"
            });
            return session.Url;
            //return new JsonResult(new { clientSecret = session.ClientSecret });
        }
        public async Task<string> PayOneTime(decimal price, string serviceName, string description, string userId)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = "http://localhost:4200/success",
                Metadata = new Dictionary<string, string>(){
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

                if (updatedInvoice.PurchaseDate != null && updatedInvoice.PurchaseDate != DateTime.MinValue )
                    existingInvoice.PurchaseDate = updatedInvoice.PurchaseDate;

                // No need to call UpdateAsync since we're modifying the tracked entity
                await invoiceRepository.SaveChangesAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
