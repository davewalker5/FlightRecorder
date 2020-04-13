using System;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;

namespace FlightRecorder.Users
{
    class Program
    {
        static void Main(string[] args)
        {
            string version = typeof(Program).Assembly.GetName().Version.ToString();
            Console.WriteLine($"Flight Recorder User Management {version}");

            CommandParser parser = new CommandParser();
            if (parser.ValidateCommandLine(args))
            {
                FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateDbContext(null);
                FlightRecorderFactory factory = new FlightRecorderFactory(context);

                try
                {
                    switch (parser.Operation)
                    {
                        case OperationType.add:
                            factory.Users.AddUser(parser.UserName, parser.Password);
                            Console.WriteLine($"Added user {parser.UserName}");
                            break;
                        case OperationType.setpassword:
                            factory.Users.SetPassword(parser.UserName, parser.Password);
                            Console.WriteLine($"Set password for user {parser.UserName}");
                            break;
                        case OperationType.delete:
                            factory.Users.DeleteUser(parser.UserName);
                            Console.WriteLine($"Deleted user {parser.UserName}");
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
                string[] commands = Enum.GetNames(typeof(OperationType));
                Console.WriteLine($"Usage : {executable} {string.Join('|', commands)} username [password]");
            }
        }
    }
}
