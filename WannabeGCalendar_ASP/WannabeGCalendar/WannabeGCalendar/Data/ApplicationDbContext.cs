using Microsoft.EntityFrameworkCore;
using WannabeGCalendar.Entities;

namespace WannabeGCalendar.Data
{
    public class ApplicationDbContext : DbContext
    {
      
        public DbSet<Users> Users { get; set; }
        public DbSet<Event> Events { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }


    }
}
