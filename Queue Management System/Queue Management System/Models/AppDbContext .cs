using FastReport.AdvMatrix;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Queue_Management_System.Models
{
    public class AppDbContext: DbContext
    {
        public DbSet<ServicePoint> ServicePoints { get; set; }
        public DbSet<QueueItem> QueueItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    
    }
}
