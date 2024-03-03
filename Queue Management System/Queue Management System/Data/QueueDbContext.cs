using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Queue_Management_System.Data
{
    public class QueueDbContext:DbContext
    {
        public QueueDbContext(DbContextOptions<QueueDbContext> options) : base(options)
        {
                
        }
    }
}
