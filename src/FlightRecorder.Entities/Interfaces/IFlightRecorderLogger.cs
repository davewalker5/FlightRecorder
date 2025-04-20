using FlightRecorder.Entities.Logging;
using System;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IFlightRecorderLogger
    {
        void Initialise(string logFile, Severity minimumSeverityToLog);
        void LogMessage(Severity severity, string message);
        void LogException(Exception ex);
    }
}
