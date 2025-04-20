using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.BusinessLogic.Logging
{
    [ExcludeFromCodeCoverage]
    public class FileLogger : IFlightRecorderLogger
    {
        private bool _configured = false;

        /// <summary>
        /// Configure logging using Serilog
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="minimumSeverityToLog"></param>
        public void Initialise(string logFile, Severity minimumSeverityToLog)
        {
            // If the log file's empty, return now without configuring a logger
            if (string.IsNullOrEmpty(logFile))
            {
                return;
            }

            // Set the minimum log level
            var levelSwitch = new LoggingLevelSwitch();
            switch (minimumSeverityToLog)
            {
                case Severity.Debug:
                    levelSwitch.MinimumLevel = LogEventLevel.Debug;
                    break;
                case Severity.Info:
                    levelSwitch.MinimumLevel = LogEventLevel.Information;
                    break;
                case Severity.Warning:
                    levelSwitch.MinimumLevel = LogEventLevel.Warning;
                    break;
                case Severity.Error:
                    levelSwitch.MinimumLevel = LogEventLevel.Error;
                    break;
                default:
                    break;
            }

            // Configure the logger
#pragma warning disable CS8602, S4792
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo
                .File(
                        logFile,
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .CreateLogger();
#pragma warning restore CS8602, S4792

            // Set the "configured" flag
            _configured = true;
        }

        /// <summary>
        /// Log a message with the specified severity
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        public void LogMessage(Severity severity, string message)
        {
            // Check the logger's been configured. If not, break out now
            if (!_configured)
            {
                return;
            }

            // Log the message
            switch (severity)
            {
                case Severity.Debug:
                    Log.Debug(message);
                    break;
                case Severity.Info:
                    Log.Information(message);
                    break;
                case Severity.Warning:
                    Log.Warning(message);
                    break;
                case Severity.Error:
                    Log.Error(message);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Log exception details, including the stack trace
        /// </summary>
        /// <param name="ex"></param>
        public void LogException(Exception ex)
        {
            // Check the logger's been configured and, if so, log the exception message and stack trace
            if (!_configured)
            {
                Log.Error(ex.Message);
                Log.Error(ex.ToString());
            }
        }
    }
}
