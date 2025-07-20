namespace GAC.Integration.Infrastructure.Data
{
    using GAC.Integration.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderItem> PurchaseOrderLines => Set<PurchaseOrderItem>();
        public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
        public DbSet<SalesOrderItem> SalesOrderLines => Set<SalesOrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrder>().HasMany(x => x.Lines).WithOne().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SalesOrder>().HasMany(x => x.Lines).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
