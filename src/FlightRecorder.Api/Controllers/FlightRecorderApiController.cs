using System.Runtime.CompilerServices;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Api.Controllers
{
    public class FlightRecorderApiController : Controller
    {
        protected FlightRecorderFactory Factory { get; private set; }
        protected IFlightRecorderLogger Logger { get; private set; }

        public FlightRecorderApiController(FlightRecorderFactory factory, IFlightRecorderLogger logger)
        {
            Factory = factory;
            Logger = logger;
        }

        /// <summary>
        /// Write a message to the log file, including the caller method name
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="caller"></param>
        protected void LogMessage(Severity severity, string message, [CallerMemberName] string caller = null)
            => Logger.LogMessage(severity, $"{caller}: {message}");
    }
}