using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightRecorder.Mvc.Api
{
    public class ExportClient : FlightRecorderClientBase
    {
        public ExportClient(HttpClient client,
            IOptions<AppSettings> settings,
            IHttpContextAccessor accessor,
            ICacheWrapper cache)
            : base(client, settings, accessor, cache)
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
            string data = JsonConvert.SerializeObject(template);
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
            string data = JsonConvert.SerializeObject(template);
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

            string data = JsonConvert.SerializeObject(template);
            await SendIndirectAsync("ExportReports", data, HttpMethod.Post);
        }

        /// <summary>
        /// Request a report export via the API using parameters held in a reporting view model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ExportReport<T>(DateBasedReportViewModelBase<T> model) where T: class
        {
            // Get the date and time
            DateTime start = !string.IsNullOrEmpty(model.From) ? DateTime.Parse(model.From) : DateTime.MinValue;
            DateTime end = !string.IsNullOrEmpty(model.To) ? DateTime.Parse(model.To) : DateTime.MaxValue;

            // Get the report definition
            var definition = ReportDefinitions.Definitions.First(x => x.EntityType == typeof(T));

            // Construct a target file name containind the date and time
            var fileName = $"{definition.FilePrefix}-{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.csv";

            // Request the export via the API
            await ExportReport(definition.ReportType, start, end, fileName);
        }
    }
}
