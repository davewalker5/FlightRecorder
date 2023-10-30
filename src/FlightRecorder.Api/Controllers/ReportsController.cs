using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly FlightRecorderFactory _factory;

        public ReportsController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Generate the airline statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("airlines/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<AirlineStatistics>>> GetAirlineStatisticsAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.AirlineStatistics.GenerateReportAsync(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }

        /// <summary>
        /// Generate the location statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("locations/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<LocationStatistics>>> GetLocationStatisticsAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.LocationStatistics.GenerateReportAsync(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }

        /// <summary>
        /// Generate the manufacturer statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("manufacturers/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<ManufacturerStatistics>>> GetManufacturerStatisticsAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.ManufacturerStatistics.GenerateReportAsync(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }

        /// <summary>
        /// Generate the aircraft model statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("models/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<ModelStatistics>>> GetModelStatisticsAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.ModelStatistics.GenerateReportAsync(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }

        /// <summary>
        /// Generate the aircraft model statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("flights/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<FlightsByMonth>>> GetFlightsByMonthAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.FlightsByMonth.GenerateReportAsync(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }

        /// <summary>
        /// Generate the job statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("jobs/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<JobStatus>>> GetJobsAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.JobStatuses
                                        .ListAsync(x => (x.Start >= startDate) && ((x.End == null) || (x.End <= endDate)),
                                                   pageNumber,
                                                   pageSize)
                                        .OrderByDescending(x => x.Start)
                                        .ToListAsync();

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results;
        }
    }
}
