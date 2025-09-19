using HomeExchange.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeExchange.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Home> Homes { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; } 
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
    }
}
