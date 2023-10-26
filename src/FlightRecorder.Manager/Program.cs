using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.DataExchange.Import;
using FlightRecorder.Entities.Reporting;
using FlightRecorder.Manager.Entities;
using FlightRecorder.Manager.Logic;
using Microsoft.EntityFrameworkCore;
using System;
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
                            IDataImporter importer = (op.EntityType == DataExchangeEntityType.sightings) ? new SightingsImporter() : new AirportImporter();
                            Task.Run(() => importer.Import(op.FileName, factory)).Wait();
                            Console.WriteLine($"Imported {op.EntityType.ToString()} data from {op.FileName}");
                            break;
                        case OperationType.export:
                            Console.WriteLine($"Exporting {op.EntityType.ToString()} to {op.FileName}");
                            Task.Run(() => Export(op.EntityType, op.FileName)).Wait();
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
                string exportEntityList = string.Join("|", Enum.GetValues<DataExchangeEntityType>());

                Console.WriteLine("\nUsage:\n");
                Console.WriteLine($"[1] {executable} add username password");
                Console.WriteLine($"[2] {executable} setpassword username password");
                Console.WriteLine($"[3] {executable} delete username");
                Console.WriteLine($"[4] {executable} import airports|sightings csv_file_path");
                Console.WriteLine($"[5] {executable} export {exportEntityList} csv_file_path");
                Console.WriteLine($"[6] {executable} update");
            }
        }

        /// <summary>
        /// Export the specified entities/report to the specified file
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task Export(DataExchangeEntityType type, string fileName)
        {
            switch (type)
            {
                case DataExchangeEntityType.sightings:
                    await ExportSightings(fileName);
                    break;
                case DataExchangeEntityType.airports:
                    await ExportAirports(fileName);
                    break;
                case DataExchangeEntityType.airlines:
                    await ExportAirlineStatistics(fileName);
                    break;
                case DataExchangeEntityType.locations:
                    await ExportLocationStatistics(fileName);
                    break;
                case DataExchangeEntityType.manufacturers:
                    await ExportManufacturerStatistics(fileName);
                    break;
                case DataExchangeEntityType.models:
                    await ExportModelStatistics(fileName);
                    break;
                case DataExchangeEntityType.flights:
                    await ExportFlightsByMonth(fileName);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Export the sightings from the database
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportSightings(string fileName)
        {
            // The third argument to the "List" method is an arbitrarily large value intended
            // to return all records
            var sightings = await factory.Sightings.ListAsync(null, 1, int.MaxValue).ToListAsync();
            SightingsExporter exporter = new();
            exporter.Export(sightings, fileName);
        }

        /// <summary>
        /// Export the airport details from the database
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportAirports(string fileName)
        {
            // The third argument to the "List" method is an arbitrarily large value intended
            // to return all records
            var airports = await factory.Airports.ListAsync(null, 1, int.MaxValue).ToListAsync();
            AirportExporter exporter = new AirportExporter();
            exporter.Export(airports, fileName);
        }

        /// <summary>
        /// Export the airline statistics report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportAirlineStatistics(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.AirlineStatistics.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<AirlineStatistics>();
            exporter.Export(records, fileName, ',');
        }

        /// <summary>
        /// Export the location statistics report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportLocationStatistics(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.LocationStatistics.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<LocationStatistics>();
            exporter.Export(records, fileName, ',');
        }

        /// <summary>
        /// Export the manufacturer statistics report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportManufacturerStatistics(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.ManufacturerStatistics.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<ManufacturerStatistics>();
            exporter.Export(records, fileName, ',');
        }

        /// <summary>
        /// Export the model statistics report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportModelStatistics(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.ModelStatistics.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<ModelStatistics>();
            exporter.Export(records, fileName, ',');
        }

        /// <summary>
        /// Export the flights by month report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static async Task ExportFlightsByMonth(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.FlightsByMonth.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<FlightsByMonth>();
            exporter.Export(records, fileName, ',');
        }
    }
}
