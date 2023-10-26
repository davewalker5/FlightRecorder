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

        public ExportController(
            IBackgroundQueue<SightingsExportWorkItem> sightingsQueue,
            IBackgroundQueue<AirportsExportWorkItem> airportsQueue)
        {
            _sightingsQueue = sightingsQueue;
            _airportsQueue = airportsQueue;
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
    }
}
