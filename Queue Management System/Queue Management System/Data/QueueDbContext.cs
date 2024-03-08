using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Queue_Management_System.Models;

namespace Queue_Management_System.Data
{
    public class QueueDbContext:IdentityDbContext
    {
        public QueueDbContext(DbContextOptions<QueueDbContext> options) : base(options)
        {
                
        }
        public DbSet<CustomerService> Customers { get; set; }
        public DbSet<Login> UserAccounts { get; set; }
    }
}
