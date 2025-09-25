using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Stripe;
using Stripe_Integration.Configurations;
using Stripe_Integration.Models;
using Stripe_Integration.Repositories;
using Stripe_Integration.Services;

namespace Stripe_Integration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddTransient<ServiceMainService>();
            builder.Services.AddTransient<InvoiceMainService>();
            builder.Services.AddTransient<PaymentService>();
            builder.Services.AddTransient<ServiceMainRepository>();
            builder.Services.AddTransient<Repository<InvoiceDetail>>();
            builder.Services.AddTransient<InvoiceMainRepository>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            builder.Services.AddDbContext<Context.StripeDbContext>(options =>
                options.UseLazyLoadingProxies()
                //.EnableSensitiveDataLogging()
                .UseSqlServer(builder.Configuration.GetConnectionString("StripeDatabase"))
            );

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<Product, ServiceMain>()
                    .AfterMap((src, dest) =>
                    {
                        dest.ShortDescription = src.Name;
                        dest.Price = src.Metadata != null && src.Metadata.ContainsKey("Price") ? decimal.Parse(src.Metadata["Price"]) : 0;
                        dest.ServiceType = src.Description;
                    });
                cfg.CreateMap<ServiceMain, ServiceMainDto>()
                    .AfterMap((src, dest) =>
                    {
                        dest.Name = src.ShortDescription;
                        dest.Id = src.ServiceMainID;
                        dest.Features = new List<string>();
                        src.ServiceDetails.ToList().ForEach(d => dest.Features.Add($"{d.MonthlyCount} {d.DetailItemDescription}"));
                    });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                //app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngular");
            app.UseAuthorization();


            app.MapControllers();
            app.MapScalarApiReference(options => options
                .WithTitle("Stripe Demo API")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                );
            app.Run();
        }
    }
}
