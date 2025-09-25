using Microsoft.EntityFrameworkCore;
using Stripe_Integration.Models;

namespace Stripe_Integration.Context
{
    public class StripeDbContext : DbContext
    {
        public StripeDbContext(DbContextOptions<StripeDbContext> options) : base(options)
        {
        }
        // Define your DbSets here, for example:
        public DbSet<ServiceMain> ServiceMain { get; set; }
        public DbSet<ServiceDetails> ServiceDetails { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<InvoiceMain> InvoiceMain { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPlan> UserPlans { get; set; }
        public DbSet<UserServiceUsage> UserServiceUsages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserServiceUsage>()
                .HasOne(u => u.UserPlan)
                .WithMany(p => p.UserServiceUsages)
                .HasForeignKey(u => u.UserPlanID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserPlan>()
                .HasOne(u => u.ServicesMain)
                .WithMany(p => p.UserPlans)
                .HasForeignKey(u => u.ServiceMainID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ServiceDetails>()
                .HasOne(u => u.ServicesMain)
                .WithMany(p => p.ServiceDetails)
                .HasForeignKey(u => u.ServiceMainID)
                .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(modelBuilder);
        }

    }
}
