using System.Diagnostics.CodeAnalysis;
using FlightRecorder.Entities.Db;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.Data
{
    [ExcludeFromCodeCoverage]
    public partial class FlightRecorderDbContext : DbContext
    {
        public virtual DbSet<Aircraft> Aircraft { get; set; }
        public virtual DbSet<Airline> Airlines { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<Sighting> Sightings { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public FlightRecorderDbContext(DbContextOptions<FlightRecorderDbContext> options) : base(options)
        {
        }
    }
}
