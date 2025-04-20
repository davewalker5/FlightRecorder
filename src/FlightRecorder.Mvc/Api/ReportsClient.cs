using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FlightRecorder.Mvc.Api
{
    public class ReportsClient : FlightRecorderClientBase
    {
        public ReportsClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return the sighting statistics report
        /// </summary>
        /// <returns></returns>
        public async Task<SightingStatistics> SightingStatisticsAsync()
        {
            // Construct the route
            string route = Settings.Value.ApiRoutes.First(r => r.Name == "SightingStatistics").Route;

            // Call the endpoint and decode the response
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var records = JsonConvert.DeserializeObject<SightingStatistics>(json, JsonSettings);

            return records;
        }

        /// <summary>
        /// Return the airline statistics report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<AirlineStatistics>> AirlineStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<AirlineStatistics>("AirlineStatistics", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the location statistics report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<LocationStatistics>> LocationStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<LocationStatistics>("LocationStatistics", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the manufacturer statistics report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<ManufacturerStatistics>> ManufacturerStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<ManufacturerStatistics>("ManufacturerStatistics", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the model statistics report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<ModelStatistics>> ModelStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<ModelStatistics>("ModelStatistics", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the flights by month report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<FlightsByMonth>> FlightsByMonthAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<FlightsByMonth>("FlightsByMonth", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the job status report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<JobStatus>> JobStatusAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<JobStatus>("JobStatus", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the "My Flights" report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<MyFlights>> MyFlightsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<MyFlights>("MyFlights", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return a date-based statistics report
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="routeName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task<List<T>> DateBasedReportAsync<T>(string routeName, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // URL encode the dates
            string fromRouteSegment = (from ?? DateTime.MinValue).ToString(Settings.Value.DateTimeFormat);
            string toRouteSegment = (to ?? DateTime.MaxValue).ToString(Settings.Value.DateTimeFormat);

            // Construct the route
            string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == routeName).Route}/{fromRouteSegment}/{toRouteSegment}/{pageNumber}/{pageSize}";

            // Call the endpoint and decode the response
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var records = JsonConvert.DeserializeObject<List<T>>(json, JsonSettings);

            return records;
        }
    }
}
