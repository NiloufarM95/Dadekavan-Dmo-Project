using System.Data.Entity;
using Flight_Detection.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Flight_Detection.DataAccess
{
    public class AppDbContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //edit connection string to connect your server
            optionsBuilder.UseSqlServer("Data Source  =.; Initial Catalog = FlightInfo; Persist Security Info=True;User ID=sa; Password= 9573");
        }

        public Microsoft.EntityFrameworkCore.DbSet<Route> Route { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Flight> Flight { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Subscription> Subscription { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>().HasNoKey();

            modelBuilder.Entity<Route>().HasKey(c => c.RouteId);

            modelBuilder.Entity<Route>()
                .HasMany(p => p.Flights)
                .WithOne(b => b.Route)
                .HasForeignKey(p => p.RouteId);

            modelBuilder.Entity<Flight>().HasKey(c => c.FlightId);
        }
    }
}
