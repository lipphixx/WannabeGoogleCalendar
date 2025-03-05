using Microsoft.EntityFrameworkCore;
using WannabeGCalendar.Entities;

namespace WannabeGCalendar.Data
{
    public class ApplicationDbContext : DbContext
    {
      
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Recovery> Recoveries { get; set; }
        public DbSet<UserLabel> UserLabels { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }


    }
}
