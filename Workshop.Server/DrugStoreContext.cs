using Microsoft.EntityFrameworkCore;

namespace Workshop.Server
{
    public class DrugStoreContext : DbContext
    {
        public DrugStoreContext()
        {
        }

        public DrugStoreContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Drug> Drugs { get; set; }

        public DbSet<DrugDeal> Deals { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Inline the Lat-Long pairs in Order rather than having a FK to another table
            modelBuilder.Entity<Order>().OwnsOne(o => o.DeliveryLocation);
        }
    }
}
