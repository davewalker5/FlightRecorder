using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Reporting;
using Microsoft.Extensions.Logging;

namespace FlightRecorder.Client.ApiClient
{
    public class ReportsClient : FlightRecorderClientBase, IReportsClient
    {
        public ReportsClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<ReportsClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return the sighting statistics report
        /// </summary>
        /// <returns></returns>
        public async Task<SightingStatistics> SightingStatisticsAsync()
        {
            // Construct the route
            string route = Settings.ApiRoutes.First(r => r.Name == "SightingStatistics").Route;

            // Call the endpoint and decode the response
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var records = Deserialize<SightingStatistics>(json);

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
            string fromRouteSegment = (from ?? DateTime.MinValue).ToString(Settings.DateTimeFormat);
            string toRouteSegment = (to ?? DateTime.MaxValue).ToString(Settings.DateTimeFormat);

            // Construct the route
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == routeName).Route}/{fromRouteSegment}/{toRouteSegment}/{pageNumber}/{pageSize}";

            // Call the endpoint and decode the response
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var records = Deserialize<List<T>>(json);

            return records;
        }
    }
}
