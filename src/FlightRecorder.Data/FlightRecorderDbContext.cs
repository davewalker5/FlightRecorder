﻿using System.Diagnostics.CodeAnalysis;
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

        public FlightRecorderDbContext(DbContextOptions<FlightRecorderDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Initialise the FlightRecorder model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Aircraft)
                    .HasForeignKey(d => d.ModelId);
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

                entity.HasOne(d => d.Airline)
                    .WithMany(p => p.Flight)
                    .HasForeignKey(d => d.AirlineId);
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

                entity.HasOne(d => d.Manufacturer)
                    .WithMany(p => p.Model)
                    .HasForeignKey(d => d.ManufacturerId);
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

                entity.HasOne(d => d.Aircraft)
                    .WithMany(p => p.Sighting)
                    .HasForeignKey(d => d.AircraftId);

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.Sighting)
                    .HasForeignKey(d => d.FlightId);

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Sighting)
                    .HasForeignKey(d => d.LocationId);
            });
        }
    }
}
