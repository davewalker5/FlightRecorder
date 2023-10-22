using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using System;
using FlightRecorder.Entities.Reporting;
using System.Collections;
using System.Linq;

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
        public async Task<ActionResult<List<AirlineStatistics>>> GetSightingsByFlightAndDateAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.AirlineStatistics.GenerateReport(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }
    }
}
