using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Manager.Entities;
using FlightRecorder.Manager.Logic;
using FlightRecorder.DataExchange;

namespace FlightRecorder.Manager
{
    static class Program
    {
        static void Main(string[] args)
        {
            string version = typeof(Program).Assembly.GetName().Version.ToString();
            Console.WriteLine($"Flight Recorder Database Management {version}");

            Operation op = new CommandParser().ParseCommandLine(args);
            if (op.Valid)
            {
                FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateDbContext(null);
                FlightRecorderFactory factory = new FlightRecorderFactory(context);

                try
                {
                    switch (op.Type)
                    {
                        case OperationType.add:
                            factory.Users.AddUser(op.UserName, op.Password);
                            Console.WriteLine($"Added user {op.UserName}");
                            break;
                        case OperationType.setpassword:
                            factory.Users.SetPassword(op.UserName, op.Password);
                            Console.WriteLine($"Set password for user {op.UserName}");
                            break;
                        case OperationType.delete:
                            factory.Users.DeleteUser(op.UserName);
                            Console.WriteLine($"Deleted user {op.UserName}");
                            break;
                        case OperationType.import:
                            CsvImporter importer = new CsvImporter();
                            importer.Import(op.FileName, factory);
                            Console.WriteLine($"Imported data from {op.FileName}");
                            break;
                        case OperationType.export:
                            // The third parameter is an arbitrary large number intended to capture all
                            // sightings
                            IEnumerable<Sighting> sightings = factory.Sightings.List(null, 1, 99999999);
                            CsvExporter exporter = new CsvExporter();
                            exporter.Export(sightings, op.FileName);
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
                Console.WriteLine($"[4] {executable} import csv_file_path");
                Console.WriteLine($"[5] {executable} export csv_file_path");
                Console.WriteLine($"[6] {executable} update");
            }
        }
    }
}
