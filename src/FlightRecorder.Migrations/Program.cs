using System;
using FlightRecorder.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateDbContext(null);
            context.Database.Migrate();
        }
    }
}
