using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Reporting;
using Microsoft.Extensions.Logging;

namespace FlightRecorder.Client.ApiClient
{
    public class ExportClient : FlightRecorderClientBase, IExportClient
    {
        public ExportClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<ExportClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Request a sightings export via the API
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportSightings(string fileName)
        {
            dynamic template = new { FileName = fileName };
            string data = Serialize(template);
            await SendIndirectAsync("ExportSightings", data, HttpMethod.Post);
        }

        /// <summary>
        /// Request an airports export via the API
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportAirports(string fileName)
        {
            dynamic template = new { FileName = fileName };
            string data = Serialize(template);
            await SendIndirectAsync("ExportAirports", data, HttpMethod.Post);
        }

        /// <summary>
        /// Request a reports export via the API
        /// </summary>
        /// <param name="type"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportReport(ReportType type, DateTime? from, DateTime? to, string fileName)
        {
            dynamic template = new
            {
                Type = (int)type,
                Start = from,
                End = to,
                FileName = fileName
            };

            string data = Serialize(template);
            await SendIndirectAsync("ExportReports", data, HttpMethod.Post);
        }

        /// <summary>
        /// Request a report export via the API using parameters held in a reporting view model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task ExportReport<T>(string from, string to) where T: class
        {
            // Get the date and time
            DateTime start = !string.IsNullOrEmpty(from) ? DateTime.Parse(from) : DateTime.MinValue;
            DateTime end = !string.IsNullOrEmpty(to) ? DateTime.Parse(to) : DateTime.MaxValue;

            // Get the report definition
            var definition = ReportDefinitions.Definitions.First(x => x.EntityType == typeof(T));

            // Construct a target file name containind the date and time
            var fileName = $"{definition.FilePrefix}-{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.csv";

            // Request the export via the API
            await ExportReport(definition.ReportType, start, end, fileName);
        }
    }
}
