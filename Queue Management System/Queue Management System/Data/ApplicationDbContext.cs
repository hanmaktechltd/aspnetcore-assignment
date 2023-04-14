using Microsoft.EntityFrameworkCore;

namespace Queue_Management_System.Models.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<CustomerTicket> Customers { get; set; }

        public DbSet<ServicePoint> ServicePoints { get; set; }

        public DbSet<ServiceProvider> ServiceProviders { get; set; }

        public DbSet<Admin> Administrator { get; set; }
    }
}

