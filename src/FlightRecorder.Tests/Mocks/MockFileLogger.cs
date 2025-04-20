using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Tests.Mocks
{
    [ExcludeFromCodeCoverage]
    public class MockFileLogger : IFlightRecorderLogger
    {
        public void Initialise(string logFile, Severity minimumSeverityToLog)
        {
        }

        public void LogMessage(Severity severity, string message)
        {
            Debug.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [{severity.ToString()}] {message}");
        }

        public void LogException(Exception ex)
        {
            LogMessage(Severity.Error, ex.Message);
            LogMessage(Severity.Error, ex.ToString());
        }
    }
}
