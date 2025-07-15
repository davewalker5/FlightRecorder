using FlightRecorder.BusinessLogic.Config;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.BusinessLogic.Logging;
using FlightRecorder.Data;
using FlightRecorder.DataExchange.Import;
using FlightRecorder.Manager.Entities;
using FlightRecorder.Manager.Logic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace FlightRecorder.Manager
{
    public static class Program
    {
        private static FlightRecorderFactory factory;
        public static void Main(string[] args)
        {
            // Output the title, including the application version
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.WriteLine($"Flight Recorder Database Management {info.FileVersion}");

            // Parse the command line
            Operation op = new CommandParser().ParseCommandLine(args);
            if (op.Valid)
            {
                // Read the application settings
                var settings = new FlightRecorderConfigReader().Read("appsettings.json", "ApplicationSettings");

                // Create a file logger
                var logger = new FileLogger();
                logger.Initialise(settings.LogFile, settings.MinimumLogLevel);

                // Create a database context and a factory class
                FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateDbContext(null);
                factory = new FlightRecorderFactory(context, logger);

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
                            IDataImporter importer = (op.EntityType == CommandLineEntityType.sightings) ? new SightingsImporter() : new AirportImporter();
                            Task.Run(() => importer.Import(op.FileName, factory)).Wait();
                            Console.WriteLine($"Imported {op.EntityType.ToString()} data from {op.FileName}");
                            break;
                        case OperationType.export:
                            Console.WriteLine($"Exporting {op.EntityType.ToString()} to {op.FileName}");
                            Task.Run(() => new DataExporter(factory).Export(op.EntityType, op.FileName)).Wait();
                            break;
                        case OperationType.update:
                            context.Database.Migrate();
                            Console.WriteLine($"Applied the latest database migrations");
                            break;
                        case OperationType.lookup:
                            Console.WriteLine($"Looking up {op.EntityType.ToString()} with identifier {op.Identifier}");
                            Task.Run(() => new EntityLookup(logger, settings.ApiEndpoints, settings.ApiServiceKeys).LookupEntity(op.EntityType, op.Identifier)).Wait();
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

                Console.WriteLine("\nUsage:\n");
                Console.WriteLine($"[1] {executable} add username password");
                Console.WriteLine($"[2] {executable} setpassword username password");
                Console.WriteLine($"[3] {executable} delete username");
                Console.WriteLine($"[4] {executable} import airports|sightings csv_file_path");
                Console.WriteLine($"[5] {executable} export sightings|airports|airlines|locations|manufacturers|flights csv_file_path");
                Console.WriteLine($"[6] {executable} update");
                Console.WriteLine($"[7] {executable} lookup flight|airport|aircraft");
            }
        }
    }
}
