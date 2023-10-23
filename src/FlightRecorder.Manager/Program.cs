using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.DataExchange;
using FlightRecorder.Entities.Db;
using FlightRecorder.Manager.Entities;
using FlightRecorder.Manager.Logic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlightRecorder.Manager
{
    public static class Program
    {
        private static FlightRecorderFactory factory;
        public static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine($"Flight Recorder Database Management {info.FileVersion}");

            Operation op = new CommandParser().ParseCommandLine(args);
            if (op.Valid)
            {
                FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateDbContext(null);
                factory = new FlightRecorderFactory(context);

                try
                {
                    switch (op.Type)
                    {
                        case OperationType.add:
                            Task.Run(() => factory.Users.AddUserAsync(op.UserName, op.Password)).Wait();
                            Console.WriteLine($"Added user {op.UserName}");
                            break;
                        case OperationType.setpassword:
                            Task.Run(() => factory.Users.SetPasswordAsync(op.UserName, op.Password)).Wait();
                            Console.WriteLine($"Set password for user {op.UserName}");
                            break;
                        case OperationType.delete:
                            Task.Run(() => factory.Users.DeleteUserAsync(op.UserName)).Wait();
                            Console.WriteLine($"Deleted user {op.UserName}");
                            break;
                        case OperationType.import:
                            IDataImporter importer = (op.EntityType == DataExchangeEntityType.sightings) ? new CsvImporter() : new AirportImporter();
                            Task.Run(() => importer.Import(op.FileName, factory)).Wait();
                            Console.WriteLine($"Imported {op.EntityType.ToString()} data from {op.FileName}");
                            break;
                        case OperationType.export:
                            if (op.EntityType == DataExchangeEntityType.sightings)
                            {
                                Task.Run(() => ExportSightings(op.FileName)).Wait();
                            }
                            else
                            {
                                Task.Run(() => ExportAirports(op.FileName)).Wait();
                            }
                            Console.WriteLine($"Exported the database to {op.FileName}");
                            break;
                        case OperationType.update:
                            context.Database.Migrate();
                            Console.WriteLine($"Applied the latest database migrations");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error : {ex.Message}");
                }
            }
            else
            {
                string executable = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("Usage:");
                Console.WriteLine($"[1] {executable} add username password");
                Console.WriteLine($"[2] {executable} setpassword username password");
                Console.WriteLine($"[3] {executable} delete username");
                Console.WriteLine($"[4] {executable} import airports|sightings csv_file_path");
                Console.WriteLine($"[5] {executable} export airports|sightings csv_file_path");
                Console.WriteLine($"[6] {executable} update");
            }
        }

        /// <summary>
        /// Export the sightings from the database
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportSightings(string fileName)
        {
            // The third argument to the "List" methods is an arbitrarily large value intended
            // to return all records
            IEnumerable<Sighting> sightings = await factory.Sightings.ListAsync(null, 1, 99999999).ToListAsync();
            CsvExporter exporter = new();
            exporter.Export(sightings, fileName);
        }

        /// <summary>
        /// Export the airport details from the database
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportAirports(string fileName)
        {
            // The third argument to the "List" methods is an arbitrarily large value intended
            // to return all records
            IEnumerable<Airport> airports = await factory.Airports.ListAsync(null, 1, 99999999).ToListAsync();
            AirportExporter exporter = new AirportExporter();
            exporter.Export(airports, fileName);
        }
    }
}
