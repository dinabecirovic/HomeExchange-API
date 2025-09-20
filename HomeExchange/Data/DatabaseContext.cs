using HomeExchange.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Advertisement)
                .WithMany()
                .HasForeignKey(r => r.AdvertisementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Advertisement>()
                .HasOne(a => a.HomeOwner)
                .WithMany() 
                .HasForeignKey(a => a.HomeOwnerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}