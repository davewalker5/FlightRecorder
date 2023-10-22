using System.Diagnostics.CodeAnalysis;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Reporting;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.Data
{
    [ExcludeFromCodeCoverage]
    public partial class FlightRecorderDbContext : DbContext
    {
        public virtual DbSet<Aircraft> Aircraft { get; set; }
        public virtual DbSet<Airline> Airlines { get; set; }
        public virtual DbSet<Airport> Airports { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<Sighting> Sightings { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<AirlineStatistics> AirlineStatistics { get; set; }
        public virtual DbSet<LocationStatistics> LocationStatistics { get; set; }
        public virtual DbSet<ManufacturerStatistics> ManufacturerStatistics { get; set; }
        public virtual DbSet<ModelStatistics> ModelStatistics { get; set; }

        public FlightRecorderDbContext(DbContextOptions<FlightRecorderDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Initialise the FlightRecorder model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AirlineStatistics>().HasNoKey();
            modelBuilder.Entity<LocationStatistics>().HasNoKey();
            modelBuilder.Entity<ManufacturerStatistics>().HasNoKey();
            modelBuilder.Entity<ModelStatistics>().HasNoKey();

            modelBuilder.Entity<Aircraft>(entity =>
            {
                entity.ToTable("AIRCRAFT");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Manufactured).HasColumnName("manufactured");

                entity.Property(e => e.ModelId).HasColumnName("model_id");

                entity.Property(e => e.Registration)
                    .IsRequired()
                    .HasColumnName("registration")
                    .HasColumnType("VARCHAR(50)");

                entity.Property(e => e.SerialNumber)
                    .IsRequired()
                    .HasColumnName("serial_number")
                    .HasColumnType("VARCHAR(50)");
            });

            modelBuilder.Entity<Airline>(entity =>
            {
                entity.ToTable("AIRLINE");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<Airport>(entity =>
            {
                entity.ToTable("AIRPORT");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CountryId).HasColumnName("country_id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("VARCHAR(5)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("COUNTRY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(50)");
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.ToTable("FLIGHT");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AirlineId).HasColumnName("airline_id");

                entity.Property(e => e.Destination)
                    .IsRequired()
                    .HasColumnName("destination")
                    .HasColumnType("VARCHAR(3)");

                entity.Property(e => e.Embarkation)
                    .IsRequired()
                    .HasColumnName("embarkation")
                    .HasColumnType("VARCHAR(3)");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasColumnName("number")
                    .HasColumnType("VARCHAR(50)");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("LOCATION");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.ToTable("MANUFACTURER");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.ToTable("MODEL");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(100)");
            });

            modelBuilder.Entity<Sighting>(entity =>
            {
                entity.ToTable("SIGHTING");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AircraftId).HasColumnName("aircraft_id");

                entity.Property(e => e.Altitude).HasColumnName("altitude");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date")
                    .HasColumnType("DATETIME");

                entity.Property(e => e.FlightId).HasColumnName("flight_id");

                entity.Property(e => e.LocationId).HasColumnName("location_id");
            });


            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("UserName")
                    .HasColumnType("VARCHAR(50)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("Password")
                    .HasColumnType("VARCHAR(1000)");
            });
        }
    }
}
