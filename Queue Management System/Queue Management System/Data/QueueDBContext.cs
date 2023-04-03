
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Models;
using Queue_Management_System.Models.ViewModels;

namespace Queue_Management_System.Data
{
    public class QueueDBContext : IdentityDbContext
    {
        public QueueDBContext(DbContextOptions<QueueDBContext> options) : base(options)
        {

        }
        public DbSet<ServicePointModel> servicepoints { get; set; }
        public DbSet<Customers> customers { get; set; }

    }
}
