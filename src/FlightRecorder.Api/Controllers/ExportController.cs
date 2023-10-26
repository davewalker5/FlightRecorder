using FlightRecorder.Api.Entities;
using FlightRecorder.BusinessLogic.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ExportController : Controller
    {
        private readonly IBackgroundQueue<SightingsExportWorkItem> _sightingsQueue;
        private readonly IBackgroundQueue<AirportsExportWorkItem> _airportsQueue;
        private readonly IBackgroundQueue<ReportExportWorkItem> _reportsQueue;

        public ExportController(
            IBackgroundQueue<SightingsExportWorkItem> sightingsQueue,
            IBackgroundQueue<AirportsExportWorkItem> airportsQueue,
            IBackgroundQueue<ReportExportWorkItem> reportsQueue)
        {
            _sightingsQueue = sightingsQueue;
            _airportsQueue = airportsQueue;
            _reportsQueue = reportsQueue;
        }

        [HttpPost]
        [Route("sightings")]
        public IActionResult ExportSightings([FromBody] SightingsExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Sightings Export";

            // Queue the work item
            _sightingsQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("airports")]
        public IActionResult ExportAirports([FromBody] AirportsExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Airports Export";

            // Queue the work item
            _airportsQueue.Enqueue(item);
            return Accepted();
        }

        [HttpPost]
        [Route("reports")]
        public IActionResult ExportReport([FromBody] ReportExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Report Export";

            // Queue the work item
            _reportsQueue.Enqueue(item);
            return Accepted();
        }
    }
}
